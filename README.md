# VRCPicSimilarity(工事中)
VRChatで撮影した写真を保存したフォルダを読み込み、時間と絵柄を考慮していい感じに写真を4枚を選んでTwitterに投稿するアプリケーションです。
Windowsのタスクスケジューラーで実行ファイルを登録すると定期的に実行できます

# 機能
- VRChatの写真フォルダ配下を日別に割り振り直す
- できるだけ最近撮影した写真を選びます
- dHashを用いた画像類似度を計算し、似ている写真は投稿写真から除かれます

# 使い方
- [Twitter開発者プラットフォーム](https://developer.twitter.com/ja/docs/twitter-ads-api/getting-started)で開発者用トークンを申請して取得する。
- `appsettings_sample.json`を参考に`appsettings.json`を作成し取得したトークンを設定する
- Windowsタスクスケジューラーに日時や実行ファイルを指定する
