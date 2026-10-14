$path = "source/repos/lesson2/lesson2/wwwroot/index.html"
$c = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)

# 1. Добавляем секцию поиска задачи перед списком задач
$searchHtml = @"
<div style='margin-top: 15px; padding: 10px; background: #e9ecef; border-radius: 4px;'>
            <input type='number' id='taskIdInput' placeholder='ID задачи' style='width: 30%;'>
            <button onclick='findTask()'>Найти задачу</button>
            <pre id='taskResult' style='margin-top: 10px;'>Результат поиска появится здесь...</pre>
        </div>
        <ul id='taskList'
"@

$c = $c.Replace("<ul id='taskList'", $searchHtml)

# 2. Добавляем функцию findTask перед window.onload
$findFunc = @"
// Поиск задачи по ID (Задача 4)
        async function findTask() {
            const id = document.getElementById('taskIdInput').value;
            if (!id) {
                alert('Введите ID задачи');
                return;
            }
            const response = await fetch(`/api/tasks/${id}`);
            const data = await response.json();
            const el = document.getElementById('taskResult');
            if (response.ok) {
                el.textContent = JSON.stringify(data, null, 2);
            } else {
                el.textContent = `Ошибка: ${data.error}`;
            }
        }

        // Автозагрузка при открытии страницы
"@

$c = $c.Replace("// Автозагрузка при открытии страницы", $findFunc)

[System.IO.File]::WriteAllText($path, $c, [System.Text.Encoding]::UTF8)
Write-Host "HTML updated successfully"
