import json
import requests

TRANSLATE_URL = "http://192.168.64.128:5001/translate"

def translateFile(inputFile, outputFile):
  # Загружаем JSON
  with open(inputFile, "r", encoding="utf-8") as f:
      data = json.load(f)

  def translate_component(component):
      ru_text = component.get("translationsNameComponent", {}).get("ru")
      if ru_text:
          response = requests.post(
              TRANSLATE_URL,
              data={
                  "q": ru_text,
                  "source": "ru",
                  "target": "en",
                  "format": "text"
              }
          )
          en_text = response.json().get("translatedText", "")
          print(ru_text + "->" + en_text)
          component["translationsNameComponent"]["en"] = en_text

      # Рекурсивно для вложенных компонентов
      for sub in component.get("components", []):
          translate_component(sub)

  # Переводим категорию
  category_ru = data.get("translationsNameCategory", {}).get("ru")
  if category_ru:
      response = requests.post(
              TRANSLATE_URL,
              data={
                  "q": category_ru,
                  "source": "ru",
                  "target": "en",
                  "format": "text"
              }
          )
      
      category_en = response.json().get("translatedText", "")
      print(category_ru + "->" + category_en)
      data["translationsNameCategory"]["en"] = category_en

  # Переводим все компоненты
  for comp in data.get("components", []):
      translate_component(comp)

  # Сохраняем результат
  with open(outputFile, "w", encoding="utf-8") as f:
      json.dump(data, f, ensure_ascii=False, indent=2)

  print("Перевод завершён! " + outputFile)

translateFile(
    "../LizeriumServerSrc/LizeriumServer/wwwroot/GameServerConfigs/BUILDS/craft_builda.json","craft_builda.json")

translateFile(
    "../LizeriumServerSrc/LizeriumServer/wwwroot/GameServerConfigs/BUILDS/craft_builde.json","craft_builde.json")

translateFile(
    "../LizeriumServerSrc/LizeriumServer/wwwroot/GameServerConfigs/BUILDS/craft_buildl.json","craft_buildl.json")

translateFile(
    "../LizeriumServerSrc/LizeriumServer/wwwroot/GameServerConfigs/BUILDS/craft_buildw.json","craft_buildw.json")