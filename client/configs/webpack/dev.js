// development config
const { merge } = require('webpack-merge');
const commonConfig = require('./common');

module.exports = merge(commonConfig, {
  mode: 'development',
  devtool: 'cheap-module-source-map',
  module: {
    rules: [
      {
        test: /\.css$/,
        use: ['style-loader', 'css-loader'],
      },
      {
        test: /\.(scss|sass)$/,
        use: ['style-loader', 'css-loader', 'sass-loader'],
      },
    ],
  },
  // https://burnedikt.com/webpack-dev-server-and-routing/
  devServer: {
    historyApiFallback: true,
    hot: true,
  },
  output: {
    publicPath: '/',
  },
});
