﻿
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.ProjectOxford.Face.Controls;

using SampleUserControlLibrary;

namespace Microsoft.ProjectOxford.Face
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constants

        private const string DefaultEndpoint = "https://westus.api.cognitive.microsoft.com/face/v1.0/";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ServicePointManager.DefaultConnectionLimit = 1000;

            // You can use the next line to insert your own subscription key, instead of using UI to set license key.
            this.ViewModel = new MainViewModel()
            {
                FaceDetectionDescription = "Locate faces in an image. You can pick an image by 'Choose Image'. Detected faces will be shown on the image by rectangles surrounding the face, and related attributes will be shown in a list.",
                FaceVerificationDescription = "There are two demos in Face Verification sample.",
                FacePersonVerificationDescription = "Face to person verification determines whether one face belongs to one person. You can pick an image folder containing one person's single face, and another single face image. Then click 'Verify' to get the verification result.",
                FaceFaceVerificationDescription = "Face to Face Verification determines whether two faces belong to the same person. You can pick single face image, the detected face will be shown on the image. Then click 'Verify' to get the verification result.",
                FaceGroupingDescription = "Put similar faces to same group according to appearance similarity. You can pick an image folder for grouping by 'Group', doing this will group all detected faces and shown under Grouping Result.",
                FaceFindSimilarDescription = "Find similar faces for the query face from all the candidates. You can pick an image folder, and all detected faces inside the folder will be treated as candidate. Use 'Open Query Face' to pick the query faces. The result will be list as 'query face's thumbnail', 'similar faces' thumbnails with similarity ranked'. Both of 'MatchPerson' mode and 'MatchFace' mode results will be listed. 'MatchPerson' mode return the top candidate faces among those recognized as the same person with the query face, so if no candidate faces are recognized as the same person with the query face, no one will be returned, while 'MatchFace' mode returns the top candidate faces with highest similarity confidence without checking if the returned face belong to the same person with the query face.",
                FaceIdentificationDescription = "Tell whom an input face belongs to given a tagged person database. Here we only handle tagged person database in following format: 1). One root folder. 2). Sub-folders are named as person's name. 3). Each person's images are put into their own sub-folder. Pick the root folder, then choose an image to identify, all faces will be shown on the image with the identified person's name.",
                FaceBearDescription = " If you want have bear. I can have you :)",
                FaceGlassesDescription = " If you want have bear. I can have you :)",
                FaceHairDescription = " If you want have bear. I can have you :)",
                FaceiconDetectionDescription = " If you want have icon. I can have you :)",
            };
            this.DataContext = this.ViewModel;
            this._scenariosControl.SampleScenarioList = new Scenario[]
            {
                new Scenario()
                {
                    PageClass = typeof(FaceDetectionPage),
                    Title = "Face Detection",
                },
        
                new Scenario()
                {
                    PageClass = typeof(FaceFindSimilarPage),
                    Title = "Face Find Similar",
                },
                new Scenario()
                {
                    PageClass = typeof(FaceGroupingPage),
                    Title = "Face Grouping",
                },
                new Scenario()
                {
                    PageClass = typeof(FaceVerificationPage),
                    Title = "Face Verification",
                },
                 new Scenario()
                {
                    PageClass = typeof(FaceBearDetectionPage),
                    Title = "Face Bear Detection",
                },
                    new Scenario()
                {
                    PageClass = typeof(FaceHairDetectionPage),
                    Title = "Face Hair Detection",
                },
                       new Scenario()
                {
                    PageClass = typeof(FaceGlassesDetectionPage),
                    Title = "Face Glasses Detection",
                },
                  new Scenario()
                {
                    PageClass = typeof(FaceiconDetectionPage),
                    Title = "Face icon Detection",
                },
            };

            // Set the default endpoint when main windows is initiated.
            var mainWindow = GetWindow(this) as MainWindow;
            mainWindow?._scenariosControl.SetSubscriptionPageEndpoint(DefaultEndpoint);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets view model instance for MainWindow
        /// </summary>
        public MainViewModel ViewModel
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Log message in main window log pane
        /// </summary>
        /// <param name="format">format string</param>
        /// <param name="args">format arguments</param>
        public static void Log(string format, params object[] args)
        {
            ((MainWindow)Application.Current.MainWindow)._scenariosControl.Log(string.Format(format, args));
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// View model for MainWindow, covers display image, text
        /// </summary>
        public class MainViewModel : INotifyPropertyChanged
        {
            internal string FaceBearDescription;
            internal string FaceGlassesDescription;
            internal string FaceHairDescription;
            #region Events

            /// <summary>
            /// Implements INotifyPropertyChanged interface
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion Events

            #region Properties

            /// <summary>
            /// Gets or sets description of face detection
            /// </summary>
            public string FaceDetectionDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of face verification
            /// </summary>
            public string FaceVerificationDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of face to face verification
            /// </summary>
            public string FaceFaceVerificationDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of face to person verification
            /// </summary>
            public string FacePersonVerificationDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of face grouping 
            /// </summary>
            public string FaceGroupingDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of find similar face
            /// </summary>
            public string FaceFindSimilarDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of identification 
            /// </summary>
            public string FaceIdentificationDescription
            {
                get;
                set;
            }
            public string FaceLipStickDescription { get; internal set; }
            public string FaceiconDetectionDescription { get; internal set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Helper function for INotifyPropertyChanged interface 
            /// </summary>
            /// <typeparam name="T">Property type</typeparam>
            /// <param name="caller">Property name</param>
            private void OnPropertyChanged<T>([CallerMemberName]string caller = null)
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(caller));
                }
            }

            #endregion Methods
        }

        #endregion Nested Types

        private void _scenariosControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}