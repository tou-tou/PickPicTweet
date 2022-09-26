// See https://aka.ms/new-console-template for more information
using PickPicTweet;

PicSimilarity ps = new PicSimilarity();
var prjPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));

//Console.WriteLine(ps.UInt64ToBinary(hash));

// ソースディレクトリ以下のpictureを日付別で割り振る
// VRChatの場合、 Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
//var vrcPicDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
var sampleDir = prjPath + @"\SamplePic";
var ped = new PicEveryDay();
ped.Distribute(sampleDir);


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
 var subFileOptions = subDi.GetFiles("*.png");
 foreach (var fi in subFileOptions)
 {
  if (sourceFiles.Count > fileCount) break;
  sourceFiles.Add(fi.FullName);
 }
}

// 最近投稿した画像をローカルテキストから取得
var outputPath = prjPath + @"\output_path.txt";
var postPaths = new List<string>();

if (!File.Exists(outputPath)) using (File.Create(outputPath)){}
using (var  sr = new StreamReader(outputPath))
{
 var line = "";
 while ((line = sr.ReadLine()) != null)
 {
  postPaths.Add(line);
 }
}
// 最近投稿した画像パスを削除
foreach (var path in postPaths)
{
 sourceFiles.Remove(path);
}

// 撮影時刻と見た目を考慮した画像を4枚選ぶ
var sbp = new SelectBalancedPic(sourceFiles);
//距離15だと似た画像が出現する
var balancedPics =sbp.Pick(20);

var tw = new Twitter();
//tw.TextTweet("test from c#");
// 画像付きツイート
tw.ImageTweet("test",balancedPics);


// 今までに投稿した画像パスを100個保存
// postPathsは時系列昇順なので前半を削る
postPaths.AddRange(balancedPics);
if (postPaths.Count > 100) postPaths = postPaths.GetRange(postPaths.Count - 100, 100);
// テキストをすべて上書き
using (var writer=new StreamWriter(prjPath+@"\output_path.txt",false))
{
 foreach (var path in postPaths)
 {
  writer.WriteLine(path);
 }
}


/*
 * 以下動作確認用のコード
 */
 
// var path1 = Path.GetFullPath(prjPath + @"\SamplePic\VRChat_1920x1080_2022-07-01_00-05-08.406.png");
// var path2 = Path.GetFullPath(prjPath + @"\SamplePic\VRChat_1920x1080_2022-07-01_23-04-43.397.png");
// var path3 = Path.GetFullPath(prjPath + @"\SamplePic\VRChat_1920x1080_2022-07-01_23-04-44.848.png");
//
// // img2とimg3は似ている画像
// var img1 = new Bitmap(path1);
// var img2 = new Bitmap(path2);
// var img3 = new Bitmap(path3);
//
// var result1 = ps.ComputeHammingDistance(img1, img2);
// var result2 = ps.ComputeHammingDistance(img2, img3);
// Console.WriteLine(result1);
// Console.WriteLine(result2);

