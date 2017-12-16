using System;
using System.Drawing;
using System.Linq;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.IO;

namespace TemplateReplacement
{
    public class TemplateReplacement
    {
        private Image<Bgr, byte> template;
        private Image<Gray, byte> templateMask;
        private string[] imagesPaths;

        public TemplateReplacement()
        {
            LoadImages();
            CreateOutputDirectoryIfNotExists(imagesPaths[0]);
            LoadTemplate();
        }

        public void Execute()
        {
            new ParallelAction<string>(Environment.ProcessorCount, ProcessImage).Execute(imagesPaths);
        }

        #region PATH_OPERATIONS

        private void LoadImages()
        {
            imagesPaths = Directory.GetFiles(@"Input");
        }

        private void LoadTemplate()
        {
            var templatePath = Directory.GetFiles(@"Logo").ToList()[0];
            template = new Image<Bgr, byte>(templatePath);
            templateMask = new Image<Gray, Byte>(template.Size).ThresholdBinaryInv(new Gray(255), new Gray(255));
        }

        private void CreateOutputDirectoryIfNotExists(string filePath)
        {
            var rootDir = ResolveOutputFilePath(filePath).Substring(0, filePath.IndexOf("\\") + 1);
            if (!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }
        }

        private string ResolveOutputFilePath(string filePath)
        {
            var folder = filePath.Replace("Input", "Output");
            return folder.Replace("input", "output");
        }

        #endregion PATH_OPERATIONS

        #region IMAGE_PROCESSING
        private void ProcessImage(string filePath)
        {
            var imgColor = CreateBGRImage(filePath);
            var imgBin = BGRToGray(imgColor);

            ProcessMatching(imgBin, imgColor, template);
            SaveImage(imgColor, ResolveOutputFilePath(filePath));
        }

        private void ProcessMatching(Image<Gray, byte> imgBin, Image<Bgr, byte> imageOrig, Image<Bgr, byte> template)
        {
            var contours = GetContours(imgBin);
            var contoursCoords = contours.ToArrayOfArray();

            foreach (var contour in contoursCoords)
            {
                if (MatchTemplateDimensions(contour, template) && IsValidContour(imgBin, contour))
                {
                    PasteLogo(imageOrig, template, contour);
                }
            }
        }

        private VectorOfVectorOfPoint GetContours(Image<Gray, byte> imgBin)
        {
            var contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(imgBin, contours, new Mat(), RetrType.Ccomp, ChainApproxMethod.ChainApproxSimple);
            return contours;
        }

        private void PasteLogo(Image<Bgr, byte> imageOrig, Image<Bgr, byte> template, Point[] contour)
        {
            imageOrig.ROI = new Rectangle(contour[0].X, contour[0].Y, template.Width, template.Height);
            CvInvoke.cvCopy(template, imageOrig, IntPtr.Zero);
            imageOrig.ROI = Rectangle.Empty;
        }

        private void SaveImage(Image<Bgr, byte> imageToSave, string filePath)
        {
            imageToSave.Save(@filePath);
        }
        #endregion IMAGE_PROCESSING

        #region IMAGE_HELPER_METHODS
        private Image<Bgr, byte> CreateBGRImage(string filePath)
        {
            return new Image<Bgr, byte>(filePath);
        }

        private Image<Gray, byte> BGRToGray(Image<Bgr, byte> imgColor)
        {
            return imgColor.Convert<Gray, byte>();
        }
        #endregion IMAGE_HELPER_METHODS

        #region VALIDATIONS
        private bool MatchTemplateDimensions(Point[] contour, Image<Bgr, byte> template)
        {
            int xDif = contour[2].X - contour[0].X + 1;
            int yDif = contour[2].Y - contour[0].Y + 1;

            return xDif == template.Cols && yDif == template.Rows;
        }

        private bool IsValidContour(Image<Gray, byte> imgBin, Point[] contour)
        {
            imgBin.ROI = new Rectangle(contour[0].X, contour[0].Y, templateMask.Width, templateMask.Height);
            Image<Gray, byte> copyROI = imgBin.Copy();
            imgBin.ROI = Rectangle.Empty;

            return copyROI.CountNonzero().FirstOrDefault() == templateMask.CountNonzero().FirstOrDefault();
        }
        #endregion VALIDATIONS
    }
}
