// See https://aka.ms/new-console-template for more information


using System.Drawing;
using VRCPicSimilarity;

PicSimilarity ps = new PicSimilarity();
var prjPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
Console.WriteLine(prjPath);

//Console.WriteLine(ps.UInt64ToBinary(hash));

var path1 = Path.GetFullPath(prjPath + @"\SamplePic\VRChat_1920x1080_2022-07-01_00-05-08.406.png");
var path2 = Path.GetFullPath(prjPath + @"\SamplePic\VRChat_1920x1080_2022-07-01_23-04-43.397.png");
var path3 = Path.GetFullPath(prjPath + @"\SamplePic\VRChat_1920x1080_2022-07-01_23-04-44.848.png");

// img2とimg3は似ている画像
var img1 = new Bitmap(path1);
var img2 = new Bitmap(path2);
var img3 = new Bitmap(path3);

var result1 = ps.ComputeHammingDistance(img1, img2);
var result2 = ps.ComputeHammingDistance(img2, img3);


Console.WriteLine(result1);
Console.WriteLine(result2);
