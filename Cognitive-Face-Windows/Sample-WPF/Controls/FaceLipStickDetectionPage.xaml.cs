using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Microsoft.ProjectOxford.Face.Controls
{
    public partial class FaceLipStickDetectionPage : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// Interaction logic for FaceLipStickDetectionPage.xaml
        /// </summary>
        /// 
        /// 
        /// <summary>
        /// Face detection results in list container
        /// </summary>
        private ObservableCollection<Face> _detectedFaces = new ObservableCollection<Face>();


        public FaceLipStickDetectionPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets face detection results
        /// </summary>



        public event PropertyChangedEventHandler PropertyChanged;

        private ImageSource selectedFiles;
        private async void Image_Click(object sender, RoutedEventArgs e)
        {
            // Create dlg type OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // sert default Text
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image files (*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif";

            // show dialog
            // C# 6.0 Question Mark mean null exception -> default constructor
            bool? result = dlg.ShowDialog();
            if (!(bool)result)
                return;
            // Display image
            string filePath = dlg.FileName;

            Uri fileUri = new Uri(filePath);
            BitmapImage bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            // upload original photo
            LipstickDisp.Source = bitmapSource;

            using (var fStream = File.OpenRead(filePath))
            {
                try
                {
                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    string subscriptionKey = mainWindow._scenariosControl.SubscriptionKey;
                    string endpoint = mainWindow._scenariosControl.SubscriptionEndpoint;

                    lbl_text.Content = "Waitting!";
                    
                    var faceServiceClient = new FaceServiceClient(subscriptionKey, endpoint);
                    ProjectOxford.Face.Contract.Face[] faces = await faceServiceClient.DetectAsync(fStream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair, FaceAttributeType.Emotion, FaceAttributeType.Hair, FaceAttributeType.Makeup, FaceAttributeType.Occlusion, FaceAttributeType.Accessories, FaceAttributeType.Noise, FaceAttributeType.Exposure, FaceAttributeType.Blur });
                                        
                    if (faces.Length > 0)
                    {
                        //Drawing around the lip 
                        DrawingVisual visual = new DrawingVisual();
                        DrawingContext drawingContext = visual.RenderOpen();
                        drawingContext.DrawImage(bitmapSource, new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                        double dpi = bitmapSource.DpiX;

                        // Set DPI image
                        double resizeFactor = (dpi == 0) ? 1 : 96 / dpi;
                        
                        foreach (var face in faces)
                        {

                            //face.FaceAttributes.Makeup.LipMakeup.ToString()
                            double UpperLipTop_X = face.FaceLandmarks.UpperLipTop.X;
                            double UpperLipTop_Y = face.FaceLandmarks.UpperLipTop.Y;
                            double UpperLipBottom_X = face.FaceLandmarks.UpperLipBottom.X;
                            double UpperLipBottom_Y = face.FaceLandmarks.UpperLipBottom.Y;

                            double UnderLipTop_X = face.FaceLandmarks.UnderLipTop.X;
                            double UnderLipTop_Y = face.FaceLandmarks.UnderLipTop.Y;
                            double UnderLipBottom_X = face.FaceLandmarks.UnderLipBottom.X;
                             double UnderLipBottom_Y = face.FaceLandmarks.UnderLipBottom.Y;
                            // lbl_text.Content = faces.Length.ToString() + ' ' + UpperLipTop_X.ToString() + ' ' + UpperLipTop_Y.ToString() + ' ' + UnderLipTop_X.ToString() + ' ' + UnderLipTop_Y.ToString();
                            //lbl_text.Content = face.FaceAttributes.Makeup.LipMakeup.ToString();
                            // Draw a rectangle on the lip
                            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 2),
                                new Rect(
                                    UpperLipTop_X * resizeFactor,
                                    UpperLipTop_Y * resizeFactor,
                                    (UpperLipBottom_X - UpperLipTop_X) * resizeFactor,
                                    (UpperLipBottom_Y - UpperLipTop_Y) * resizeFactor
                                )
                            );
                            // Loop through the images pixels to reset color.

                            //lbl_text.Content = UpperLipTop_X; //* resizeFactor;
                            //drawingContext.DrawRectangle(Brushes.Red, null, 
                            //    new Rect(
                            //        UpperLipTop_X * resizeFactor - 50, UpperLipTop_Y * resizeFactor, 50, 20
                            //    )
                            //);

                            //int i = Int32.Parse((UpperLipTop_X * resizeFactor - 100) * 100);
                            //for (double Y = UpperLipTop_Y * resizeFactor - 100; Y <= UnderLipBottom_Y * resizeFactor + 100; Y++)
                            //{
                            //    for (double X = UpperLipTop_X * resizeFactor - 100; X < UpperLipTop_X * resizeFactor + 100; X++)
                            //    {
                            //        drawingContext.DrawRectangle(Brushes.Red, null, new Rect(Y, X, 1, 1));
                            //    }
                            //}
                            //lbl_text.Content = count.ToString() + ' ' + bitmapSource.Width.ToString() + ' ' + bitmapSource.Height.ToString();
                        }
                        
                        // Close drawing 
                        drawingContext.Close();

                        

                        // Display the image with the rectangle around the face
                        RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                            (int)(bitmapSource.PixelWidth * resizeFactor),
                            (int)(bitmapSource.PixelHeight * resizeFactor),
                            96, 96, PixelFormats.Pbgra32);

                        faceWithRectBitmap.Render(visual);
                        lipstickDisplay.Source = faceWithRectBitmap;
                    }

                }
                catch (FaceAPIException ex)
                {
                    MainWindow.Log("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                    GC.Collect();
                    return;
                }
            }
        }
       
    }
}
