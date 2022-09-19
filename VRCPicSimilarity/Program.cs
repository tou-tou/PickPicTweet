// See https://aka.ms/new-console-template for more information


using System.Drawing;
using VRCPicSimilarity;

PicSimilarity ps = new PicSimilarity();
var prjPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
Console.WriteLine(prjPath);

//Console.WriteLine(ps.UInt64ToBinary(hash));

// ソースディレクトリ以下のpictureを日付別で割り振る
// VRChatの場合、 Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
var vrcPicDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
var ped = new PicEveryDay();
//ped.Distribute(vrcPicDir);


/*
 * ソースディレクトリ配下の最近の画像100枚を取得
 */

const int fileCount = 100;
// ディレクトリ直下のpngファイルを取得
var di = new DirectoryInfo(prjPath+@"\SamplePic");
// サブディレクトリを含むすべてのファイル一覧の場合はオプションに、"SearchOption.AllDirectories"を追加
var fileOptions = di.GetFiles("*.png");
var sourceFiles = new List<string>();
foreach (var fileInfo in fileOptions) {
 sourceFiles.Add(fileInfo.FullName);
}
// ソースディレクトリ直下のディレクトリ直下のpngファイルを取得
var subDirNames = new List<string>();
var subDirs = di.GetDirectories();
foreach (var dir in subDirs)
{
 subDirNames.Add(dir.FullName);
}
subDirNames.Sort();
subDirNames.Reverse();

foreach (var dirName in subDirNames)
{
 if (sourceFiles.Count > fileCount) break;
 var subDi = new DirectoryInfo(dirName);
 var subFileOptions = di.GetFiles("*.png");
 foreach (var fi in subFileOptions)
 {
  if (sourceFiles.Count > fileCount) break;
  sourceFiles.Add(fi.FullName);
 }
}


// 最近投稿した画像を取り除く(twitter orローカルテキストから)

// 撮影時刻と見た目を考慮した画像を4枚選ぶ
var sbp = new SelectBalancedPic(sourceFiles);
var balancedPics =sbp.Pick();

var tw = new Twitter();
//tw.TextTweet("test from c#");
// 画像付きツイート
tw.ImageTweet("test",balancedPics);

/*
 * 以下動作確認用のコード
 */
 
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

