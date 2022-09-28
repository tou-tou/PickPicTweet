// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using PickPicTweet;

// 実行時引数の処理
// --full-path 
// --is-everyday-dir
// --pic-pool-count
// --no-tweet
var argIndexFullPath = Array.IndexOf(args, "--full-path");
var argIndexEveryDir = Array.IndexOf(args, "--every-dir");
var argIndexCount = Array.IndexOf(args, "--pic-pool-count");
var argIndexNoTweet = Array.IndexOf(args, "--no-tweet");

/*
 * 設定ファイルの読み込み
 */
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

/*
 * 画像を含むソースディレクトリを指定
 */
// デフォルトはVRChatを想定
var sourceDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
var flag1
 = argIndexFullPath > 0 && argIndexFullPath + 1 < args.Length && Directory.Exists(args[argIndexFullPath+1]);
if (flag1)
{
 sourceDir = args[argIndexFullPath + 1];
}

/*
 * ソースディレクトリ以下の画像を月別または日付別で割り振る
 * flagが0の時は機能off,1の時は月別,2の時は日付別
 * デフォルトは0
 */
var flag2 = argIndexEveryDir > 0 && argIndexEveryDir + 1 < args.Length &&
            Int32.TryParse(args[argIndexEveryDir + 1],out _);
if (flag2)
{
 Console.WriteLine(args[argIndexEveryDir + 1]);
 var ped = new PicEveryDay();
 ped.Distribute(sourceDir,Int32.Parse(args[argIndexEveryDir + 1]));
}

/*
 * ソースディレクトリ配下の最近の画像100枚を取得し画像の候補を挙げる
 */
var fileCount = 100;
var flag3 = argIndexCount > 0 && argIndexCount + 1 < args.Length && int.TryParse(args[argIndexCount+1],out var resultCount);
if (flag3)
{
 fileCount = int.Parse(args[argIndexCount + 1]);
}

// サブディレクトリを含むすべてのpngファイルパスを取得して作成日で並び替え
var filePaths = Directory.GetFiles(sourceDir, "*.png", SearchOption.AllDirectories)
 .Where(filePath => true /* 特定のファイルは除く */).OrderBy(filePath => File.GetCreationTime(filePath).Date).Reverse().ToList();

filePaths = filePaths.GetRange(0, fileCount);

// 最近投稿した画像をローカルテキストから取得
var outputPath = @"output_path.txt";
var postPaths = new List<string>();

if (!File.Exists(outputPath)) using (File.Create(outputPath)){}
using (var  sr = new StreamReader(outputPath))
{
 string line;
 while ((line = sr.ReadLine()) != null)
 {
  postPaths.Add(line);
 }
}
// 最近投稿した画像パスを削除
foreach (var path in postPaths)
{
 filePaths.Remove(path);
}

/*
 * 撮影時刻と見た目を考慮した画像を最大4枚選ぶ
 */
var sbp = new SelectBalancedPic(filePaths);
// さらに類似画像を弾きたい場合は、Pickの引数:thresholdを大きくする(最大64)
var balancedPics =sbp.Pick(20);

/*
 * ツイート
 */
var tw = new Twitter(config["ConsumerKey"],config["ConsumerKeySecret"],config["AccessToken"],config["AccessTokenSecret"]);
//tw.TextTweet("test from c#");
// 画像付きツイート
var flag4 =  argIndexNoTweet > 0 && argIndexNoTweet + 1 < args.Length && args[argIndexNoTweet+1]=="1";
if (flag4)
{
 foreach (var pic in balancedPics)
 {
  Console.WriteLine(pic);
 }
}
else
{
 tw.ImageTweet("最近の！", balancedPics);
}


/*
 * 今までに投稿した画像パスを100個保存
 */
// postPathsは時系列昇順なので前半を削る
postPaths.AddRange(balancedPics);
if (postPaths.Count > 100) postPaths = postPaths.GetRange(postPaths.Count - 100, 100);
// テキストをすべて上書き
using (var writer=new StreamWriter(@"output_path.txt",false))
{
 foreach (var path in postPaths)
 {
  writer.WriteLine(path);
 }
}


/*
 * 以下動作確認用のコード
 */
 
//Console.WriteLine(args[0]);
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

