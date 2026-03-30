import json
import requests

TRANSLATE_URL = "http://192.168.64.128:5001/translate"

def translateFile(inputFile, outputFile):
    with open(inputFile, "r", encoding="utf-8") as f:
        data = json.load(f)

    for entry in data:
        if "categories" in entry:
            # если categories это строка, делаем массив для унификации
            if isinstance(entry["categories"], str):
                categories_list = [entry["categories"]]
            elif isinstance(entry["categories"], list):
                categories_list = entry["categories"]
            else:
                continue

            translated_categories = {}
            for cat in categories_list:
                response = requests.post(
                    TRANSLATE_URL,
                    data={
                        "q": cat,
                        "source": "en",
                        "target": "ru",
                        "format": "text"
                    }
                )
                ru_text = response.json().get("translatedText", "")
                translated_categories[cat] = ru_text
                print(cat + " -> " + ru_text)

            # Заменяем поле categories на объект с ru/en
            entry["categories"] = {
                "ru": list(translated_categories.values()),
                "en": categories_list
            }

    # Сохраняем результат
    with open(outputFile, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

    print("Готово! Сохранено в " + outputFile)


# Пример вызова
translateFile(
    "..\\LizeriumServerSrc\\LizeriumServer\\wwwroot\\payloads\\limit-breaking.json",
    "limit-breaking.json"
)
