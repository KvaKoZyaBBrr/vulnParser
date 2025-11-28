Утилита для создания csv файла импорта tfs из csv выгрузки АК-ВС 3.

Пример использования:
```
vulnParser.exe -i D:\defects.csv -o D:\result.csv -g true
```
Input:
```
-i, --input     Required. Input file.
-o, --output    Required. Output file.
-g, --group     Must be group by cwe type. (true/false)
```
Настроечные файлы:
- defaults.json

    Структура:
    ```
    {
    "Author" : "",//имя пользователя в tfs
    "Tags":[],//теги
    "Analyzer":"",//имя анализатора
    "ModuleMask":"",//маска(startWith) по которой будет находиться имя модуля
    "ReplaceTraceValues"://данные для замены пути в location
        [
            {
                "Original":"", // что заменять 
                "ReplaceValue":"", // чем заменять
                "Suffix":"" // суффикс
            }
        ]
    }
    ```
    Описание: настроечные параметры которые будут применены ко всем дефектам. 
    
    Пример:
    ```
    {
    "Author" : "Тест Тестов <TestDomain\\t.testov>", 
    "Tags":["Tag1","Tag2"], 
    "Analyzer":"Test",
    "ModuleMask":"TestModule.",
    "ReplaceTraceValues":
        [
            {
                "Original":"Extracted/dev",
                "ReplaceValue":"https://tfs.ru/tfs/Collection/Project/_git/rep1?path=",
                "Suffix":"&version=GBdev&_a=contents"
            }
        ]
    }
    ```

- ParentWorkerItem.txt

    Структура соответсвует заголовку
    ```
    ID,Work Item Type,Title 1,Title 2,Assigned To,State,Tags,Repro Steps,System Info
    ```
    Структура:
    ```
    ,,,,,,,,
    ```
    Пример - корневой WorkItem SAST с ID 77777, под которым будут создаваться все результаты:
    ```
    77777,User Story,SAST,,,New,,,
    ```

Для вышеописанных примеров будут составляены следующие строки:
```
ID,Work Item Type,Title 1,Title 2,Assigned To,State,Tags,Repro Steps,System Info
77777,User Story,SAST,,,New,,,
,Bug,,"Признак плохого качества кода","Тест Тестов <TestDomain\\t.testov>",New,"Tag1;Tag2","Статический анализатор выявил программную ошибку в коде модуля TestModule.Blob<br><br><b>Описание программной ошибки: </b><br>Признак плохого качества кода.<br>Ветви условного оператора индентичны<br><br><b>Расположение: </b><br><a href=""https://tfs.ru/tfs/Collection/Project/_git/rep1?path=/src/Blob/NpgsqlLargeObjectStream.cs&version=GBdev&_a=contents"">Extracted/eos-storage/dev_1.7/src/Eos.Platform.Storage.Blob/Npgsql/NpgsqlLargeObjectStream.cs</a><br>273:if (_manager.Has64BitSupport)<br><br><a href=""https://tfs.ru/tfs/Collection/Project/_git/rep1?path=/src/Blob/NpgsqlLargeObjectStream.cs&version=GBdev&_a=contents"">Extracted/dev/src/Blob/Npgsql/NpgsqlLargeObjectStream.cs</a><br>275:else<br><br><b>Оценка от Security Champion: </b><br>","Статический анализатор АК-ВС 3.<br>Хэш программной ошибки: c7349e087616d26da5a40c78615d6ce3<br>Тип ошибки: CWE-398<br>Уровень: <b>Низкий:0.2</b><br>"
,Bug,,"Проблемы, связанные с выражениями","Тест Тестов <TestDomain\\t.testov>",New,"Tag1;Tag2","Статический анализатор выявил программную ошибку в коде модуля TestModule.Blob<br><br><b>Описание программной ошибки: </b><br>Проблемы, связанные с выражениями.<br>Левая и правая части бинарного выражения идентичны<br><br><b>Расположение: </b><br><a href=""https://tfs.ru/tfs/Collection/Project/_git/rep1?path=/src/Blob/BlobApiAmazonS3Processor.cs&version=GBdev&_a=contents"">Extracted/dev/src/Blob/AmazonS3/BlobApiAmazonS3Processor.cs</a><br>39:var (stream, contentType) = await _s3Service.ReadFileAsync(uri.AmazonS3ObjectKey, S3Directory,<br><br><b>Оценка от Security Champion: </b><br>","Статический анализатор АК-ВС 3.<br>Хэш программной ошибки: 4a6e7dc81f4daf971c5c7b31988d06c8<br>Тип ошибки: CWE-569<br>Уровень: <b>Низкий:0.2</b><br>"
```

Допустимо создание defaults.Development.json и ParentWorkerItem.Development.txt для хранения рабочих настроек.
