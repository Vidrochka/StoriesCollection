# StoriesCollection
Бот с небольшими интерактивными историями


# Развертывание:
 - Создать телаграмм-бота
 - Скопировать токен бота в StoriesCollection/appsettings.json/BotConfiguration/BotToken    
 - Выполнить ```docker-compose up``` в дирректории проекта    
 - В рамках тестирования запустить ngrok полная строка: ```ngrok http 8443``` (ngrok прокидывает запусы с своего url, на наш localhost с портом, url сгенерированный ngrok нужен ниже)    
 - Сделать POST запрос (https://api.telegram.org/bot\${bottoken}/setWebhook?url=\${url}/api/bot/update), где \${bottoken} - апи бота (см. п2), \${url} - <b>https</b> url из ngrok (см. п4)    

# Конфигурирование

Для редакторивания историй на порту 5555 запущен pgAdmin4    
 - логин: pgadmin4@pgadmin.org    
 - пароль: admin    

Коннект к базе данных:    
 - логин: admin    
 - password: password    
 - название базы: stories    

База данных состоит из 3 моделей

```json

//Story
{
    "Id": "id истории",
    "Name": "Назвагие истории",
    "FirstStoryPartId": "id первой части истории"
}

// StoryPart
{
    "Id": "id части истории (строка, т.к. так проще ориентироваться)",
    "StoryId": "id истории",
    "Text": "Текст части итории"
}

//Button
Button {
    "Id": "id истории",
    "Text": "Текст кнопки",
    "SourceStoryPartId": "id части истории на которой покажется кнопка",
    "DestioationStoryPart": "id части истории на которую ведет кнопка"
}

```

*База данных сохраняется в папке db*
