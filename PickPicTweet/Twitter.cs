using CoreTweet;

namespace PickPicTweet;

/// <summary>
/// Twitterをする
/// </summary>
public class Twitter
{
    private string ConsumerKey { get; }
    private string ConsumerKeySecret { get; }
    private string AccessToken { get; }
    private string AccessTokenSecret { get; }
    private Tokens Token { get; }
    
    /// <summary>
    /// Twitter APIを利用するためのトークンを受取る
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerKeySecret"></param>
    /// <param name="accessToken"></param>
    /// <param name="accessTokenSecret"></param>
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
    /// 画像をアップロードしてメディアidを取得
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private long UploadImage(string path)
    {
        MediaUploadResult result = Token.Media.Upload(media: new FileInfo(path));
        return result.MediaId;
    }

    /// <summary>
    /// 最大4枚の画像付きツイート
    /// 画像がない場合はテキストのみツイート
    /// </summary>
    /// <param name="text"></param>
    /// <param name="paths"></param>
    public void ImageTweet(string text, List<string> paths)
    {
        //画像をアップロードして返ってきたmedia_idを追加する
        List<long> mediaIds = paths.Select(UploadImage).ToList();
        Token.Statuses.Update(new
        {
            status = text,
            media_ids = mediaIds
        });
    }
}