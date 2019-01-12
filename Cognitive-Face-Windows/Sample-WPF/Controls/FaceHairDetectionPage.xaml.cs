
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace Microsoft.ProjectOxford.Face.Controls
{
    /// <summary>
    /// Count to image
    /// </summary>
    public partial class FaceHairDetectionPage : Page, INotifyPropertyChanged
    {

        #region Fields

        /// <summary>
        /// Description dependency property
        /// </summary>
        private static int saveImage_Count = 0;

        /// <summary>
        /// Description dependency property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FaceHairDetectionPage));

        /// <summary>
        /// Face detection results in list container
        /// </summary>
        private ObservableCollection<Face> _detectedFaces = new ObservableCollection<Face>();

        /// <summary>
        /// Face detection results in text string
        /// </summary>
        private string _detectedResultsInText;

        /// <summary>
        /// Face detection results container
        /// </summary>
        private ObservableCollection<Face> _resultCollection = new ObservableCollection<Face>();

        /// <summary>
        /// Image used for rendering and detecting
        /// </summary>
        private ImageSource _selectedFile;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceHairDetectionPage" /> class
        /// </summary>
        public FaceHairDetectionPage()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets description
        /// </summary>
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }

            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets face detection results
        /// </summary>
        public ObservableCollection<Face> DetectedFaces
        {
            get
            {
                return _detectedFaces;
            }
        }

        /// <summary>
        /// Gets or sets face detection results in text string
        /// </summary>
        public string DetectedResultsInText
        {
            get
            {
                return _detectedResultsInText;
            }

            set
            {
                _detectedResultsInText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DetectedResultsInText"));
                }
            }
        }

        /// <summary>
        /// Gets constant maximum image size for rendering detection result
        /// </summary>
        public int MaxImageSizes
        {
            get
            {
                return 300;
            }
        }

        /// <summary>
        /// Gets face detection results
        /// </summary>
        public ObservableCollection<Face> ResultCollection
        {
            get
            {
                return _resultCollection;
            }
        }

        /// <summary>
        /// Gets or sets image for rendering and detecting
        /// </summary>
        public ImageSource SelectedFile
        {
            get
            {
                return _selectedFile;
            }

            set
            {
                _selectedFile = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedFile"));
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Pick image for face detection and set detection result to result container
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event argument</param>
        private async void Hair_Click(object sender, RoutedEventArgs e)
        {
            // Show file picker dialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image files (*.jpg, *.png, *.bmp, *.gif) | *.jpg; *.png; *.bmp; *.gif";
            var result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                // User picked one image
                var pickedImagePath = dlg.FileName;
                var renderingImage = UIHelper.LoadImageAppliedOrientation(pickedImagePath);
                var imageInfo = UIHelper.GetImageInfoForRendering(renderingImage);
                SelectedFile = renderingImage;

                // Clear last detection result
                ResultCollection.Clear();
                DetectedFaces.Clear();
                DetectedResultsInText = string.Format("Detecting...");

                MainWindow.Log("Request: Detecting {0}", pickedImagePath);
                var sw = Stopwatch.StartNew();



                // Call detection REST API
                using (var fStream = File.OpenRead(pickedImagePath))
                {
                    try
                    {
                        MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                        string subscriptionKey = mainWindow._scenariosControl.SubscriptionKey;
                        string endpoint = mainWindow._scenariosControl.SubscriptionEndpoint;

                        var faceServiceClient = new FaceServiceClient(subscriptionKey, endpoint);
                        ProjectOxford.Face.Contract.Face[] faces = await faceServiceClient.DetectAsync(fStream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair, FaceAttributeType.Emotion, FaceAttributeType.Hair, FaceAttributeType.Makeup, FaceAttributeType.Occlusion, FaceAttributeType.Accessories, FaceAttributeType.Noise, FaceAttributeType.Exposure, FaceAttributeType.Blur });
                        MainWindow.Log("Response: Success. Detected {0} face(s) in {1}", faces.Length, pickedImagePath);

                        DetectedResultsInText = string.Format("{0} face(s) has been detected", faces.Length);


                        //Create Image type
                        Image renderingImage_img = Image.FromFile(dlg.FileName);
                        foreach (var face in faces)
                        {
                            // add Glasses to face

                            string.Format("GlassesType: {0}", face.FaceAttributes.FacialHair.Beard.ToString());


                            if (face.FaceAttributes.FacialHair.Beard >= 0 && face.FaceAttributes.FacialHair.Beard <= 0.4)
                            {
                                // Gan icon len
                                renderingImage_img = Add_Beard(renderingImage_img, face);
                                //ImageGlassesDisplay.Source = (BitmapImage)renderingImage_bit;
                            }
                        }
                        renderingImage_img.Save("F:\\study\\Project\\New pj\\Cognitive-Face-Windows\\Images\\rabbit\\" + saveImage_Count.ToString() + ".jpg");
                        BitmapImage DisplayImage = new BitmapImage(new Uri("F:\\study\\Project\\New pj\\Cognitive-Face-Windows\\Images\\rabbit\\" + saveImage_Count.ToString() + ".jpg"));
                        ImageHairDisplay.Source = DisplayImage;
                        saveImage_Count++;



                        // Convert detection result into UI binding object for rendering
                        foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSizes, imageInfo))
                        {
                            ResultCollection.Add(face);
                        }
                    }
                    catch (FaceAPIException ex)
                    {
                        MainWindow.Log("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                        GC.Collect();
                        return;
                    }
                    GC.Collect();
                }
            }
        }

        private Image Add_Beard(Image Display_Image, ProjectOxford.Face.Contract.Face face)
        {
            //
            Image rabbit_Image = Image.FromFile("F:\\study\\Project\\New pj\\Cognitive-Face-Windows\\Data\\taitho.png");
            double x1 = face.FaceLandmarks.EyeLeftTop.X;
            double x2 = face.FaceLandmarks.EyeRightTop.X;
            double y1 = face.FaceLandmarks.EyeLeftTop.Y;
            double y2 = face.FaceLandmarks.EyeRightTop.Y;
            double ab = y2 - y1;
            double ac = x2 - x1;
            double bc = Math.Sqrt(ab * ab + ac * ac);
            double tanACB = ab / ac;
            var deg = Math.Atan(tanACB) * 180 / Math.PI;
            var RabbitWidth = bc;
            double scale = RabbitWidth / rabbit_Image.Width;
            double x = face.FaceLandmarks.EyebrowLeftInner.X;
            double moveY = (face.FaceLandmarks.EyebrowLeftInner.Y - face.FaceLandmarks.EyebrowRightInner.Y) / 2;
            double y = face.FaceLandmarks.EyebrowLeftInner.Y - moveY;
            if (deg < 0)
            {
                deg = 360 + deg;
            }







            //Rotate Image
            Image BeardImage_Rotate = rotateImage(rabbit_Image, deg);


            // Resize Glasses Image and save to New_GlassesImage
            Image BeardImage_Resize = ResizeImage(BeardImage_Rotate, scale);


            //MainWindow.Log("GlassesImage_Resize: {0}", GlassesImage_Resize.Width);
            //New_GlassesImage.Save("F:\\study\\Project\\Cognitive-Face-Windows\\Images\\Glasses\\3.png");

            //set toa do dat kinh
            int X = Convert.ToInt32(x - ac /4);
            int Y = Convert.ToInt32(y - bc * 2.5);

            // merge Glasses to Display_Image
            Display_Image = MergeImage(Display_Image, BeardImage_Resize, X, Y);
            return Display_Image;
        }


        //rotate image
        private Image rotateImage(Image img, double deg)
        {

            Image BeardImage_Rotate = (Image)(new Bitmap(img.Width, img.Height));
            using (Graphics g = Graphics.FromImage(BeardImage_Rotate))
            {
                //move rotation point to center of image
                g.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
                //rotate
                g.RotateTransform((float)deg);
                //move image back
                g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                //draw passed in image onto graphics object
                g.DrawImage(img, new System.Drawing.Point(0, 0));
            }
            return BeardImage_Rotate;
        }

        //Resize Image
        private Image ResizeImage(Image img, double scale)
        {
            int NewBeardImage_width = Convert.ToInt32(img.Width * scale);
            int NewBeardImage_Height = Convert.ToInt32(img.Height * scale);
            //Create Image
            Image Image_Resize = (Image)(new Bitmap(NewBeardImage_width, NewBeardImage_Height));

            using (Graphics g = Graphics.FromImage(Image_Resize))
            {
                g.DrawImage(img, 0, 0, NewBeardImage_width, NewBeardImage_Height);
            }

            return Image_Resize;
        }


        //Merge two image
        private Image MergeImage(Image img1, Image img2, int X, int Y)
        {
            using (Graphics g = Graphics.FromImage(img1))
            {
                //g.DrawImage(GlassesImage_Resize, X, Y);
                g.DrawImage(img2, X, Y);
            }
            return img1;
        }

        #endregion Methods


    }
}