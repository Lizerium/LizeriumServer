/// <binding BeforeBuild='all' ProjectOpened='watch' /> //запускает задачу наблюдения при открытии проекта, все задачи перед билдом (выбирается в меню Task Runner)
module.exports = function (grunt) {
  grunt.initConfig({
    clean: ["wwwroot/css/*", "wwwroot/js/app.min.js", "ScriptsAndCss/Combined/*"], //очистка файлов какие папки/файлы очищать
    sass: {
      css: {
        options: {
          implementation: require("sass"),
          sourceMap: false
        },
        files: [{
          expand: true,
          cwd: "ScriptsAndCss/CssFiles",
          src: ["**/*.scss"],
          dest: "ScriptsAndCss/Combined/scss",
          ext: ".css"
        }]
      }
    },
    ts: {
      api: {
        tsconfig: "./tsconfig.json"
      }
    },
    concat: {
      js: { //объединение JS
        src: [
          "ScriptsAndCss/JsScripts/**/*.js"
        ], //сюда можно писать файлы для объединения через запятую
        dest: "ScriptsAndCss/Combined/combined.js" //расположение объединенного файла
      },
      css: { //объединение CSS
        src: [
          "ScriptsAndCss/Combined/scss/setup.css",
          "ScriptsAndCss/Combined/scss/style.css",
          "ScriptsAndCss/Combined/scss/pages/**/*.css"
        ], //сюда можно писать файлы для объединения через запятую
        dest: "ScriptsAndCss/Combined/combined.css" //расположение объединенного файла
      }
    },
    uglify: { //сжатие JS
      js: {
        src: ["ScriptsAndCss/Combined/combined.js"], //какой файл сжимать
        dest: "wwwroot/js/app.min.js" //сжатый выходной файл
      }
    },
    cssmin: { //сжатие CSS
      css: {
        src: ["ScriptsAndCss/Combined/combined.css"], //какой файл сжимать
        dest: "wwwroot/css/app.min.css" //сжатый выходной файл
      }
    },
    watch: { //наблюдение за изменениями
      files: ["ScriptsAndCss/TypeScripts/**/*.ts", "ScriptsAndCss/JsScripts/**/*.js", "ScriptsAndCss/CssFiles/**/*.scss"], //за изменением каких файлов наблюдаем
      tasks: ["all"] //какую задачу запускаем
    }
  });

  grunt.loadNpmTasks("grunt-contrib-clean"); //для очистки файлов
  grunt.loadNpmTasks("grunt-sass");
  grunt.loadNpmTasks("grunt-contrib-concat"); //для объединения JS и CSS
  grunt.loadNpmTasks("grunt-contrib-uglify"); //для сжатия JS
  grunt.loadNpmTasks("grunt-contrib-cssmin"); //для сжатия CSS
  grunt.loadNpmTasks("grunt-ts"); //для компиляции TypeScript
  grunt.registerTask("all", ["clean", "sass", "ts", "concat", "uglify", "cssmin"]); //общая задача
  grunt.loadNpmTasks("grunt-contrib-watch"); //для наблюдения за изменениями в файлах
};
