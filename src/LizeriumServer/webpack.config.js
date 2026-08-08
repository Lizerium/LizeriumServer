const path = require('path')
const MiniCssExtractPlugin = require('mini-css-extract-plugin')
const CssMinimizerPlugin = require('css-minimizer-webpack-plugin')
const TerserPlugin = require('terser-webpack-plugin') // Импортируем TerserPlugin

module.exports = {
	entry: {
		app: './ScriptsAndCss/TypeScripts/main_api.ts', // Точка входа для JavaScript
		global: './ScriptsAndCss/CssFiles/main.scss', // Точка входа для SCSS (может быть любой SCSS-файл)
		doc_hook: './ScriptsAndCss/CssFiles/pages/documents/doc_hook.scss', // Точка входа для SCSS (может быть любой SCSS-файл)
		wiki: './ScriptsAndCss/CssFiles/knowledge/knowledge.scss', // Точка входа для SCSS (может быть любой SCSS-файл)
		wish: './ScriptsAndCss/CssFiles/pages/home/wish.scss', // Точка входа для SCSS (может быть любой SCSS-файл)
		news: './ScriptsAndCss/CssFiles/pages/home/news.scss', // Точка входа для SCSS (может быть любой SCSS-файл)
	},
	output: {
		path: path.resolve(__dirname, 'wwwroot/js'),
		filename: '[name].min.js', // Используем [name] для динамического имени файла
	},
	resolve: {
		extensions: ['.ts', '.js', '.css'], // Добавили .css
	},
	module: {
		rules: [
			{
				test: /\.ts$/,
				use: 'ts-loader',
				exclude: /node_modules/,
			},
			{
				test: /\.css$/,
				use: [
					MiniCssExtractPlugin.loader, // Извлекает CSS в отдельные файлы
					'css-loader', // Обрабатывает @import и url()
				],
			},
			{
				test: /\.scss$/,
				use: [MiniCssExtractPlugin.loader, 'css-loader', 'sass-loader'],
			},
		],
	},
	plugins: [
		new MiniCssExtractPlugin({
			filename: '../css/[name].min.css', // Куда Webpack должен поместить CSS-файл
		}),
	],
	// Debug
	//mode: 'development',
	//watch: true,
	// Release
	mode: 'production', // Изменен режим на production для минимизации
	optimization: {
		splitChunks: {
			chunks: 'all',
		},
		minimize: true, // Включаем минимизацию
		minimizer: [
			new TerserPlugin({ parallel: false }), // Плагин для минимизации JS
			new CssMinimizerPlugin({ parallel: false }), // Плагин для минимизации CSS
		],
	},
}
