const webpackConfig = require('./webpack.config.js');

module.exports = function (grunt) {
    grunt.initConfig({
        webpack: {
            options: webpackConfig,
            build: {
            }
        },
        browserSync: {
            dev: {
                bsFiles: {
                    src: [
                        'wwwroot/css/*.css',
                        'wwwroot/js/app.min.js', // Webpack создает app.min.js
                        'Views/**/*.cshtml'
                    ]
                },
                options: {
                    watchTask: true,
                    proxy: "localhost:5000" // Замените на свой локальный адрес
                }
            }
        },
        watch: {
            ts: {
                files: ['ScriptsAndCss/TypeScripts/**/*.ts'],
                tasks: ['webpack:build'], // Webpack компилирует и собирает
                options: {
                    spawn: false,
                },
            },
            bsReload: {
                files: ['wwwroot/css/*.css', 'wwwroot/js/app.min.js', 'Views/**/*.cshtml'],
                options: {
                    reload: true
                }
            }
        },
        clean: ["wwwroot/css/*", "wwwroot/js/*", "ScriptsAndCss/Combined/*"],
    });

    grunt.loadNpmTasks('grunt-webpack');
    grunt.loadNpmTasks('grunt-browser-sync');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-cssmin');

    grunt.registerTask("build", ["clean", "webpack:build"]);
    grunt.registerTask("default", ["build", "browserSync:dev", "watch"]);
};