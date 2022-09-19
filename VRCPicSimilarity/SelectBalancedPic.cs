using System.Numerics;

namespace VRCPicSimilarity;

/// <summary>
/// 最大100枚の画像から最近撮られた10枚を選び1枚(A)ランダムに取り出す
/// ImageSetを時系列の降順にソート
/// Aからthreshold以上のImageSetをつくる
/// ImageSetからランダムにBを選ぶ
/// Bからthreshold以上のImageSetを作る
/// ImageSetからランダムにCを選ぶ
/// Cからthreshold以上のImageSetを作る
/// ImageSetからランダムにDを選ぶ
/// 途中でImageSetの大きさが0になるか4毎選ぶかで終了
/// </summary>
public class SelectBalancedPic
{
    private string[] _paths;

    public SelectBalancedPic(params string[] paths)
    {
        _paths = paths;
    }
}