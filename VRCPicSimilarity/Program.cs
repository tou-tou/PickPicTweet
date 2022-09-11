// See https://aka.ms/new-console-template for more information


using System.Drawing;
using VRCPicSimilarity;

PicSimilarity ps = new PicSimilarity();
var prjPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
Console.WriteLine(prjPath);
var path = Path.GetFullPath(prjPath + @"\SamplePic\4.png");
Console.WriteLine(path);
Bitmap img = new Bitmap(path);
var format = img.RawFormat;
var reducedImg = ps.ReduceSize(img);
var grayImg = ps.ReduceColor(reducedImg);
var hash = ps.ComputeAdjacentPixelDiff(grayImg);
Console.WriteLine(ps.UInt64ToBinary(hash));
grayImg.Save(prjPath + @"\output\3.png",format);

