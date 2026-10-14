$path = "source/repos/lesson2/lesson2/wwwroot/index.html"
$c = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)

# Заменяем тег ul (с двойными кавычками!)
$ulTag = '<ul id="taskList" style="margin-top: 15px;"></ul>'
$searchDiv = @"
<div style="margin-top: 15px; padding: 10px; background: #e9ecef; border-radius: 4px;">
            <input type="number" id="taskIdInput" placeholder="ID задачи" style="width: 30%;">
            <button onclick="findTask()">Найти задачу</button>
            <pre id="taskResult" style="margin-top: 10px;">Результат поиска появится здесь...</pre>
        </div>
        <ul id="taskList" style="margin-top: 15px;"></ul>
"@
$c = $c.Replace($ulTag, $searchDiv)
Write-Host ("Step 3 fixed (search div) applied: " + $c.Contains("taskIdInput"))

[System.IO.File]::WriteAllText($path, $c, [System.Text.Encoding]::UTF8)
Write-Host "File saved"
