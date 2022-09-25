using CoreTweet;
using Microsoft.Extensions.Configuration;

namespace VRCPicSimilarity;

public class Twitter
{
    private string ConsumerKey { get; set; }
    private string ConsumerKeySecret { get; set; }
    private string AccessToken { get; set; }
    private string AccessTokenSecret { get; set; }
    private Tokens Token { get; set; }

    public Twitter()
    {
        SetKey();
        CreateToken();
    }

    private void SetKey()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        ConsumerKey = config["ConsumerKey"];
        ConsumerKeySecret = config["ConsumerKeySecret"];
        AccessToken = config["AccessToken"];
        AccessTokenSecret = config["AccessTokenSecret"];
    }

    private void CreateToken()
    {
        Token = Tokens.Create(
            ConsumerKey, ConsumerKeySecret, AccessToken, AccessTokenSecret);
    }

    public void TextTweet(string text)
    {
        Token.Statuses.Update(status => text);
    }

    /// <summary>
    /// 最大4枚の画像付きツイート
    /// </summary>
    /// <param name="text"></param>
    /// <param name="paths"></param>
    public void ImageTweet(string text, List<string> paths)
    {
        //画像をアップロードして帰ってきたmedia_idを追加する
        var mediaIds = paths.Select(UploadImage).ToList();
        Token.Statuses.Update(new
        {
            status = text,
            media_ids = mediaIds
        });
    }
    
    /// <summary>
    /// 画像をアップロードしてメディアidを取得
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private long UploadImage(string path)
    {
        var result = Token.Media.Upload(media: new FileInfo(path));
        return result.MediaId;
    }
}