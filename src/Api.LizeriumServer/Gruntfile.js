/// <binding BeforeBuild='all' ProjectOpened='watch' /> //Р·Р°РїСѓСЃРєР°РµС‚ Р·Р°РґР°С‡Сѓ РЅР°Р±Р»СЋРґРµРЅРёСЏ РїСЂРё РѕС‚РєСЂС‹С‚РёРё РїСЂРѕРµРєС‚Р°, РІСЃРµ Р·Р°РґР°С‡Рё РїРµСЂРµРґ Р±РёР»РґРѕРј (РІС‹Р±РёСЂР°РµС‚СЃСЏ РІ РјРµРЅСЋ Task Runner)
module.exports = function (grunt) {
  grunt.initConfig({
    clean: ["wwwroot/css/*", "wwwroot/js/app.min.js", "ScriptsAndCss/Combined/*"], //РѕС‡РёСЃС‚РєР° С„Р°Р№Р»РѕРІ РєР°РєРёРµ РїР°РїРєРё/С„Р°Р№Р»С‹ РѕС‡РёС‰Р°С‚СЊ
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
      js: { //РѕР±СЉРµРґРёРЅРµРЅРёРµ JS
        src: [
          "ScriptsAndCss/JsScripts/**/*.js"
        ], //СЃСЋРґР° РјРѕР¶РЅРѕ РїРёСЃР°С‚СЊ С„Р°Р№Р»С‹ РґР»СЏ РѕР±СЉРµРґРёРЅРµРЅРёСЏ С‡РµСЂРµР· Р·Р°РїСЏС‚СѓСЋ
        dest: "ScriptsAndCss/Combined/combined.js" //СЂР°СЃРїРѕР»РѕР¶РµРЅРёРµ РѕР±СЉРµРґРёРЅРµРЅРЅРѕРіРѕ С„Р°Р№Р»Р°
      },
      css: { //РѕР±СЉРµРґРёРЅРµРЅРёРµ CSS
        src: ["ScriptsAndCss/CssFiles/**/*.css", "ScriptsAndCss/Combined/scss/**/*.css"], //СЃСЋРґР° РјРѕР¶РЅРѕ РїРёСЃР°С‚СЊ С„Р°Р№Р»С‹ РґР»СЏ РѕР±СЉРµРґРёРЅРµРЅРёСЏ С‡РµСЂРµР· Р·Р°РїСЏС‚СѓСЋ
        dest: "ScriptsAndCss/Combined/combined.css" //СЂР°СЃРїРѕР»РѕР¶РµРЅРёРµ РѕР±СЉРµРґРёРЅРµРЅРЅРѕРіРѕ С„Р°Р№Р»Р°
      }
    },
    uglify: { //СЃР¶Р°С‚РёРµ JS
      js: {
        src: ["ScriptsAndCss/Combined/combined.js"], //РєР°РєРѕР№ С„Р°Р№Р» СЃР¶РёРјР°С‚СЊ
        dest: "wwwroot/js/app.min.js" //СЃР¶Р°С‚С‹Р№ РІС‹С…РѕРґРЅРѕР№ С„Р°Р№Р»
      }
    },
    cssmin: { //СЃР¶Р°С‚РёРµ CSS
      css: {
        src: ["ScriptsAndCss/Combined/combined.css"], //РєР°РєРѕР№ С„Р°Р№Р» СЃР¶РёРјР°С‚СЊ
        dest: "wwwroot/css/app.min.css" //СЃР¶Р°С‚С‹Р№ РІС‹С…РѕРґРЅРѕР№ С„Р°Р№Р»
      }
    },
    watch: { //РЅР°Р±Р»СЋРґРµРЅРёРµ Р·Р° РёР·РјРµРЅРµРЅРёСЏРјРё
      files: ["ScriptsAndCss/TypeScripts/**/*.ts", "ScriptsAndCss/JsScripts/**/*.js", "ScriptsAndCss/CssFiles/**/*.css", "ScriptsAndCss/CssFiles/**/*.scss"], //Р·Р° РёР·РјРµРЅРµРЅРёРµРј РєР°РєРёС… С„Р°Р№Р»РѕРІ РЅР°Р±Р»СЋРґР°РµРј
      tasks: ["all"] //РєР°РєСѓСЋ Р·Р°РґР°С‡Сѓ Р·Р°РїСѓСЃРєР°РµРј
    }
  });

  grunt.loadNpmTasks("grunt-contrib-clean"); //РґР»СЏ РѕС‡РёСЃС‚РєРё С„Р°Р№Р»РѕРІ
  grunt.loadNpmTasks("grunt-sass");
  grunt.loadNpmTasks("grunt-contrib-concat"); //РґР»СЏ РѕР±СЉРµРґРёРЅРµРЅРёСЏ JS Рё CSS
  grunt.loadNpmTasks("grunt-contrib-uglify"); //РґР»СЏ СЃР¶Р°С‚РёСЏ JS
  grunt.loadNpmTasks("grunt-contrib-cssmin"); //РґР»СЏ СЃР¶Р°С‚РёСЏ CSS
  grunt.loadNpmTasks("grunt-ts"); //РґР»СЏ РєРѕРјРїРёР»СЏС†РёРё TypeScript
  grunt.registerTask("all", ["clean", "sass", "ts", "concat", "uglify", "cssmin"]); //РѕР±С‰Р°СЏ Р·Р°РґР°С‡Р°
  grunt.loadNpmTasks("grunt-contrib-watch"); //РґР»СЏ РЅР°Р±Р»СЋРґРµРЅРёСЏ Р·Р° РёР·РјРµРЅРµРЅРёСЏРјРё РІ С„Р°Р№Р»Р°С…
};
