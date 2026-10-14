$path = "source/repos/lesson2/lesson2/wwwroot/index.html"
$c = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)

# 1. Заменяем headers в addTask (точный текст из файла)
$old1 = "headers: { 'Content-Type': 'application/json' },"
$new1 = "headers: { 'Content-Type': 'application/json', 'X-Api-Key': 'lesson2-secret' },"
$c = $c.Replace($old1, $new1)
Write-Host ("Step 1 (addTask headers) applied: " + $c.Contains("X-Api-Key"))

# 2. Заменяем DELETE запрос (точный текст)
$old2 = "{ method: 'DELETE' }"
$new2 = "{ method: 'DELETE', headers: { 'X-Api-Key': 'lesson2-secret' } }"
$c = $c.Replace($old2, $new2)
Write-Host ("Step 2 (deleteTask headers) applied: " + $c.Contains("lesson2-secret"))

# 3. Добавляем поиск задачи перед списком (используем тег <ul)
$ulTag = "<ul id='taskList' style='margin-top: 15px;'></ul>"
$searchDiv = "<div style='margin-top: 15px; padding: 10px; background: #e9ecef; border-radius: 4px;'>" + [char]10 + "            <input type='number' id='taskIdInput' placeholder='ID задачи' style='width: 30%;'>" + [char]10 + "            <button onclick='findTask()'>Найти задачу</button>" + [char]10 + "            <pre id='taskResult' style='margin-top: 10px;'>Результат поиска появится здесь...</pre>" + [char]10 + "        </div>" + [char]10 + "        " + $ulTag
$c = $c.Replace($ulTag, $searchDiv)
Write-Host ("Step 3 (search div) applied: " + $c.Contains("taskIdInput"))

# 4. Добавляем функцию findTask перед window.onload
$onloadStr = "window.onload"
$findTaskFunc = "async function findTask() {" + [char]10 + "            const id = document.getElementById('taskIdInput').value;" + [char]10 + "            if (!id) { alert('Введите ID задачи'); return; }" + [char]10 + "            const response = await fetch('/api/tasks/' + id);" + [char]10 + "            const data = await response.json();" + [char]10 + "            const el = document.getElementById('taskResult');" + [char]10 + "            if (response.ok) { el.textContent = JSON.stringify(data, null, 2); }" + [char]10 + "            else { el.textContent = 'Ошибка: ' + data.error; }" + [char]10 + "        }" + [char]10 + [char]10 + "        " + $onloadStr
$c = $c.Replace($onloadStr, $findTaskFunc)
Write-Host ("Step 4 (findTask func) applied: " + $c.Contains("async function findTask"))

[System.IO.File]::WriteAllText($path, $c, [System.Text.Encoding]::UTF8)
Write-Host "File saved successfully"
