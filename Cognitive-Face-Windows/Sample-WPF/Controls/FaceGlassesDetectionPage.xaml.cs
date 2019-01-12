
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
    public partial class FaceGlassesDetectionPage : Page, INotifyPropertyChanged
    {

        #region Fields

        /// <summary>
        /// Description List face Detection
        /// </summary>
        private ProjectOxford.Face.Contract.Face[] faces;

        /// <summary>
        /// Description Open File Dialog
        /// </summary>
        private Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

        /// <summary>
        /// Description Count to Image
        /// </summary>
        private static int saveImage_Count = 0;

        /// <summary>
        /// Description dependency property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FaceGlassesDetectionPage));

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
        /// Initializes a new instance of the <see cref="FaceGlassesDetectionPage" /> class
        /// </summary>
        public FaceGlassesDetectionPage()
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



        private async void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            // Show file picker dialog
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
                ImageGlassesDisplay.Source = null;
                DetectedResultsInText = string.Format("Detecting...");

                // show Main Windows
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
                        faces = await faceServiceClient.DetectAsync(fStream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair, FaceAttributeType.Emotion, FaceAttributeType.Hair, FaceAttributeType.Makeup, FaceAttributeType.Occlusion, FaceAttributeType.Accessories, FaceAttributeType.Noise, FaceAttributeType.Exposure, FaceAttributeType.Blur });
                        MainWindow.Log("Response: Success. Detected {0} face(s) in {1}", faces.Length, pickedImagePath);

                        DetectedResultsInText = string.Format("{0} face(s) has been detected. You can see the result", faces.Length);

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
        /// <summary>
        /// Pick image for face detection and load glasses for image
        /// </summary>
        /// 
        private void Glasses_Click(object sender, RoutedEventArgs e)
        {
            if(DetectedResultsInText.Equals("Detecting..."))
            {
                return;
            }

            if (!dlg.FileName.Equals(""))
            {    //Create Image type
                Image renderingImage_img = Image.FromFile(dlg.FileName);
                foreach (var face in faces)
                {
                    // add Glasses to face
                    string.Format("GlassesType: {0}", face.FaceAttributes.Glasses.ToString());

                    if (face.FaceAttributes.Glasses.ToString().Equals("NoGlasses"))
                    {
                        // Gan kinh len
                        renderingImage_img = Add_Glasses(renderingImage_img, face);
                    }
                }
                renderingImage_img.Save("F:\\study\\Project\\New pj\\Cognitive-Face-Windows\\Images\\saveFile\\" + saveImage_Count.ToString() + ".jpg");
                BitmapImage DisplayImage = new BitmapImage(new Uri("F:\\study\\Project\\New pj\\Cognitive-Face-Windows\\Images\\saveFile\\" + saveImage_Count.ToString() + ".jpg"));
                ImageGlassesDisplay.Source = DisplayImage;
                saveImage_Count++;
            }
            else
            {
                DetectedResultsInText = "Please upload a photo";
            }
        }

        /// <summary>
        /// Add Glasses to Image
        /// </summary>
        private Image Add_Glasses(Image Display_Image, ProjectOxford.Face.Contract.Face face)
        {
            //
            Image Glasses_Image = Image.FromFile("F:\\study\\Project\\New pj\\Cognitive-Face-Windows\\Images\\Glasses\\1.png");

            // Get Position Eye Left
            double EyeLeftTop_X = face.FaceLandmarks.EyeLeftTop.X;          // using
            double EyeLeftTop_Y = face.FaceLandmarks.EyeLeftTop.Y;
            double EyeLeftBottom_X = face.FaceLandmarks.EyeLeftBottom.X;
            double EyeLeftBottom_Y = face.FaceLandmarks.EyeLeftBottom.Y;
            double EyeLeftInner_X = face.FaceLandmarks.EyeLeftInner.X;      // using
            double EyeLeftInner_Y = face.FaceLandmarks.EyeLeftInner.Y;      // using
            double EyeLeftOuter_X = face.FaceLandmarks.EyeLeftOuter.X;      // using
            double EyeLeftOuter_Y = face.FaceLandmarks.EyeLeftOuter.Y;

            // Get Position Eye Right
            double EyeRightTop_X = face.FaceLandmarks.EyeRightTop.X;        
            double EyeRightTop_Y = face.FaceLandmarks.EyeRightTop.Y;
            double EyeRightBottom_X = face.FaceLandmarks.EyeRightBottom.X;
            double EyeRightBottom_Y = face.FaceLandmarks.EyeRightBottom.Y;
            double EyeRightInner_X = face.FaceLandmarks.EyeRightInner.X;    // using
            double EyeRightInner_Y = face.FaceLandmarks.EyeRightInner.Y;    // using
            double EyeRightOuter_X = face.FaceLandmarks.EyeRightOuter.X;    // using
            double EyeRightOuter_Y = face.FaceLandmarks.EyeRightOuter.Y;

            // Get Width Left eye to Right eye
            double Eyes_Width = Math.Sqrt((EyeRightOuter_X - EyeLeftOuter_X) * (EyeRightOuter_X - EyeLeftOuter_X) + (EyeRightOuter_Y - EyeRightOuter_Y) * (EyeRightOuter_Y - EyeRightOuter_Y));
            
            // Get Glasses Width = Eyes_Width + EyeRightOuter_X + EyeLeftOuter_X
            double Glasses_Width = Eyes_Width + (EyeRightOuter_X - EyeRightInner_X) + (EyeLeftInner_X - EyeLeftOuter_X);

            // get scale Glasses Width vs Glasses Original
            double ratio = Glasses_Width / Glasses_Image.Width;

            // Lấy Góc quay
            // Cạnh đối và cạnh kề của tam giác Vuông ABC, vuông tại B
            double AC = EyeRightInner_X - EyeLeftInner_X;
            double AB = EyeLeftInner_Y - EyeRightInner_Y;
            // Cạnh huyền = căn tổng bình phương 2 cạnh
            Double BC = Math.Sqrt(AB * AB + AC * AC);
            double tanACB = AB / AC;
            // Qui đổi radian sang độ
            double angel = Math.Atan(tanACB) * 180 / Math.PI;

            //Rotate Image
            Image GlassesImage_Rotate = rotateImage(Glasses_Image, angel);



            // Resize Glasses Image and save to New_GlassesImage
            Image GlassesImage_Resize = ResizeImage(GlassesImage_Rotate, ratio);

            //set toa do dat kinh
            // avg eye
            double avgEye_X = (EyeLeftInner_X + EyeRightInner_X) / 2;
            double avgEye_Y = Math.Abs((EyeLeftInner_Y + EyeRightInner_Y) / 2);

            //Toa do dat kinh
            int X = Convert.ToInt32(avgEye_X - (GlassesImage_Resize.Width / 2));
            int Y = Convert.ToInt32(avgEye_Y - (GlassesImage_Resize.Height / 2));
           
            // merge Glasses to Display_Image
            Display_Image = MergeImage(Display_Image, GlassesImage_Resize, X, Y);
            return Display_Image;
        }


        //rotate image
        private Image rotateImage(Image img, double angle)
        {
            // create New Image
            Image img_Rotate = (Image)(new Bitmap(Convert.ToInt32(img.Width), Convert.ToInt32(img.Height)));

            using (Graphics g = Graphics.FromImage(img_Rotate))
            {
                //move rotation point to center of image
                g.TranslateTransform(img_Rotate.Width / 2, img_Rotate.Height / 2);
                //rotate
                g.RotateTransform(- (float)angle);
                //move image back
                g.TranslateTransform( -(img_Rotate.Width / 2), -(img_Rotate.Height / 2));
                //draw passed in image onto graphics object.hei
                g.DrawImage(img, new System.Drawing.Point(0, 0));
            }
            img_Rotate.Save("F:\\study\\Project\\New pj\\Cognitive-Face-Windows\\Images\\saveFile\\glasses.png");
            // return Image Rotated
            return img_Rotate;
        }

        //Resize Image
        private Image ResizeImage(Image img, double ratio)
        {
            
            int NewImg_Width = Convert.ToInt32(img.Width * ratio);
            int NewImg_Height = Convert.ToInt32(img.Height * ratio);
            //Create Image
            Image Img_Resize = (Image)(new Bitmap(NewImg_Width, NewImg_Height));

            using (Graphics g = Graphics.FromImage(Img_Resize))
            {
                g.DrawImage(img, 0, 0, NewImg_Width, NewImg_Height);
            }
            
            //return image resized
            return Img_Resize;
        }


        //Merge two image
        private Image MergeImage(Image img1, Image img2, int X, int Y)
        {
            using (Graphics g = Graphics.FromImage(img1))
            {
                //g.DrawImage(GlassesImage_Resize, X, Y);
                g.DrawImage(img2, X, Y);
            }
            //return image merged
            return img1;
        }

        #endregion Methods
    }
}