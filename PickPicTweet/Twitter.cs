using CoreTweet;
using Microsoft.Extensions.Configuration;

namespace PickPicTweet;

public class Twitter
{
    private string ConsumerKey { get; }
    private string ConsumerKeySecret { get; }
    private string AccessToken { get; }
    private string AccessTokenSecret { get; }
    private Tokens Token { get; }

    public Twitter(string consumerKey, string consumerKeySecret, string accessToken, string accessTokenSecret)
    {
        ConsumerKey = consumerKey;
        ConsumerKeySecret = consumerKeySecret;
        AccessToken = accessToken;
        AccessTokenSecret = accessTokenSecret;
        Token = Tokens.Create(
            ConsumerKey, ConsumerKeySecret, AccessToken, AccessTokenSecret);
    }
    /// <summary>
    /// テキストのみツイート
    /// </summary>
    /// <param name="text"></param>
    public void TextTweet(string text)
    {
        Token.Statuses.Update(status => text);
    }

    /// <summary>
    /// 最大4枚の画像付きツイート
    /// 画像がない場合はテキストのみツイート
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