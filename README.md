# PickPicTweet
VRChat等で撮影した写真を保存したフォルダを読み込み、時間と絵柄を考慮していい感じに写真を4枚を選んでTwitterに投稿するアプリケーションです。
Windowsのタスクスケジューラーで実行ファイルを登録すると定期的に実行できます

### 解説記事

- [VRSNSで撮った写真を自動でいい感じに選択して定期的にTwitterに投稿できるようにしてみた - Qiita](https://qiita.com/__tou__tou/items/f61ce2f86e92f08c1738)

# 機能
- VRChatの写真フォルダ配下を日別に割り振り直す(デフォルトでは機能オフ)
- いい感じに写真を選びます
  - できるだけ最近撮影した写真を選びます
  - 過去に選ばれた写真は除かれます
  - [dHash](https://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html) を用いた画像類似度を計算し、似ている写真は投稿写真から除かれます
  - secretフォルダ直下の写真は選ばれない
    - 手動でsecretフォルダを作成し、その直下に選びたくない写真を予め入れておく必要があります
- TwitterAPI用のトークンを利用してTweetする
# 環境
- Windows10
- .NET6.0

# 使い方
## リリースページから実行ファイルをダウンロード
- [Twitter開発者プラットフォーム](https://developer.twitter.com/ja/docs/twitter-ads-api/getting-started) で開発者用トークンを申請して取得する。
- [Releases · tou-tou/PickPicTweet](https://github.com/tou-tou/PickPicTweet/releases) から`PickPicTweet.exe`と`appsettings.json`をダウンロード
- `appsettings.json`に取得したトークンを設定し、実行ファイルと同じディレクトリに配置
- Windowsタスクスケジューラーに日時や実行ファイルを指定する

## VisualStudioで実行ファイルを生成
- [Twitter開発者プラットフォーム](https://developer.twitter.com/ja/docs/twitter-ads-api/getting-started) で開発者用トークンを申請して取得する。
- `PickPicTweet/appsettings_sample.json`を参考に`PickPicTweet/appsettings.json`を作成し取得したトークンを設定する
- VisualStudioでソリューションをビルドし、実行ファイルを生成する
  - .NET6.0の環境です
- Windowsタスクスケジューラーに日時や実行ファイルを指定する

## 実行時オプション
- `--full-path "画像が含まれるフォルダのフルパス"`
  - デフォルトはVRChatの画像フォルダへのパス
- `--every-dir n`
  - 0:デフォルト値、何もしない 
  - 1:画像を月別のフォルダに振り分ける。一度実行するともとに戻せないので注意が必要。
  - 2:画像を月別フォルダ配下の日付別フォルダに振り分ける。一度実行するともとに戻せないので注意が必要。
- `--pic-pool-count n`
  - n枚の写真から4枚が選ばれる。デフォルトは100
- `--no-tweet n`
  - 0:ツイートする、デフォルト値
  - 1:ツイートしない

