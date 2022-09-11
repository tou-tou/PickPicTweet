using System.Drawing;

namespace VRCPicSimilarity;

/// <summary>
/// 2つの画像の類似度をdHashを用いて計算する
/// dHash:
/// https://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html
/// https://tech.unifa-e.com/entry/2017/11/27/111546
/// </summary>

public class PicSimilarity
{
    /// <summary>
    /// 高速に処理するために画像サイズを縮小する
    /// デフォルトサイズは9×8=72px
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    public Bitmap ReduceSize(Bitmap img)
    {
        return new Bitmap(img, 9, 8);
    }
    
    /// <summary>
    /// 画像をグレースケールに変換
    /// グレースケールの変換式は　Y=0.2126R+0.7152G+0.0722B
    /// ref:https://ja.wikipedia.org/wiki/%E3%82%B0%E3%83%AC%E3%83%BC%E3%82%B9%E3%82%B1%E3%83%BC%E3%83%AB
    /// Bitmapクラスは画像のピクセルデータと属性で構成される
    /// ref:https://docs.microsoft.com/ja-jp/dotnet/api/system.drawing.bitmap?view=dotnet-plat-ext-6.0
    /// </summary>
    /// <returns></returns>
    public Bitmap ReduceColor(Bitmap img)
    {
        int h = img.Height;
        int w = img.Width;

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Color c = img.GetPixel(i, j);
                Byte gamma = (byte) (c.R*0.2126 + c.G*0.7152 + c.B*0.0722);
                img.SetPixel(i, j, Color.FromArgb(gamma, gamma, gamma));
            }
        }
        return img;
    }
    /// <summary>
    /// 9(width)×8(height)画像で、先頭からはじめて、右隣のピクセルの輝度が高ければ1のフラグを立てる
    /// フラグを並べていくと64bitになる
    /// </summary>
    /// <param name="img"></param>
    /// <returns></returns>
    public ulong ComputeAdjacentPixelDiff(Bitmap img)
    {
        ulong hash = 0b_0;
        int w = img.Width;
        int h = img.Height;
        
        // 高さを固定
        for (int j = 0; j < h; j++)
        {
            // 横のピクセルの隣接数はw-1
            for (int i = 0; i < w-1; i++)
            {
                if (img.GetPixel(i,j).R < img.GetPixel(i+1,j).R)
                {
                    hash = (hash|1) << 1;
                }
                else
                {
                    hash = hash << 1;
                }
            }
        }
        return hash;
    }
    /// <summary>
    /// ulong(uint64)をバイナリで表示する
    /// ビット列の最下位ビットを取り出し、1か0かを判定して文字列を後ろから詰める
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public string UInt64ToBinary(ulong v)
    {
        System.Text.StringBuilder b = new System.Text.StringBuilder();
        for (int i = 0; i < 64; i++)
        {
            b.Insert(0,((v&1)==1) ? "1":"0");
            v >>= 1;
        }
        return b.ToString();
    }
}