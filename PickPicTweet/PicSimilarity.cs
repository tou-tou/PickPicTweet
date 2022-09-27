using System.Drawing;
using System.Numerics;

namespace PickPicTweet;

/// <summary>
/// 2つの画像の類似度をdHashを用いて計算する
/// それぞれの画像をdHashでハッシュ化し、そのハッシュのハミング距離をとる
/// ref:https://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html
/// 図解:https://tech.unifa-e.com/entry/2017/11/27/111546    
/// </summary>
public class PicSimilarity
{
    /// <summary>
    /// 画像サイズを縮小する
    /// デフォルトサイズは9×8=72px
    /// </summary>
    /// <param name="img"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public Bitmap ReduceSize(Bitmap img,int width=9,int height=8)
    {
        return new Bitmap(img, width, height);
    }
    
    /// <summary>
    /// 画像をグレースケールに変換
    /// グレースケールの変換式は　Y=0.2126R+0.7152G+0.0722B
    /// ref:https://ja.wikipedia.org/wiki/%E3%82%B0%E3%83%AC%E3%83%BC%E3%82%B9%E3%82%B1%E3%83%BC%E3%83%AB
    /// </summary>
    /// <returns></returns>
    public Bitmap ConvertGray(Bitmap img)
    {
        int w = img.Width;
        int h = img.Height;
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
                    hash = hash<<1|1;//左シフトしてから最下位ビットに1を追加
                }
                else
                {
                    hash<<=1;//左シフトで最下位ビットには0が追加される
                }
            }
        }
        return hash;
    }
    
    /// <summary>
    /// ulong(uint64)をバイナリの文字列にする
    /// ビット列の最下位ビットを取り出し、1か0かを判定して文字列を後ろから詰める
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public string UInt64ToBinary(ulong v)
    {
        System.Text.StringBuilder b = new System.Text.StringBuilder();
        for (int i = 0; i < 64; i++)
        {
            b.Insert(0,((v&1)==1) ? "1":"0");//最下位ビットを取り出し後ろから文字をつめる
            v >>= 1;
        }
        return b.ToString();
    }
    
    /// <summary>
    /// ulong(uint64)のハミング距離[0,64]を計算
    /// 二つの画像のハッシュ(uint64)をXORしてPopCount(いくつのビットが1かをカウントする)
    /// XORとPopCountはどちらもプリミティブな命令なので高速に計算できる
    /// ref:https://ja.wikipedia.org/wiki/%E3%83%8F%E3%83%9F%E3%83%B3%E3%82%B0%E8%B7%9D%E9%9B%A2
    /// </summary>
    /// <param name="img1"></param>
    /// <param name="img2"></param>
    /// <returns></returns>
    public int ComputeHammingDistance(Bitmap img1,Bitmap img2)
    {
        //画像サイズを小さくして、グレースケールに変換し、ハッシュ化
        var hash1 = ComputeAdjacentPixelDiff(ConvertGray(ReduceSize(img1)));
        var hash2 = ComputeAdjacentPixelDiff(ConvertGray(ReduceSize(img2)));
        //ハミング距離の計算
        return BitOperations.PopCount(hash1^hash2);
    }
}