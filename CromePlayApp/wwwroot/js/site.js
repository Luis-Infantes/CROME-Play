// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



// LOGICA PARA GESTION EL CAMBIO DE COLOR DEL BACKGROUND

document.addEventListener("DOMContentLoaded", () => {
    // Colores válidos que se relacionan con los de las clases predefinidas
    const THEMES = ['white', 'blue', 'green', 'yellow', 'gray'];

    // Elimina solo clases de tema del body
    const removeThemeClasses = () => {
        THEMES.forEach(c => document.body.classList.remove(c));
    };

    // Aplica una clase de tema al body
    const applyTheme = (klass) => {
        if (!klass || !THEMES.includes(klass)) return;
        removeThemeClasses();
        document.body.classList.add(klass);
        // Notificamos por si otros scripts quisieran reaccionar
        document.dispatchEvent(new CustomEvent('theme:changed', { detail: { selectedClass: klass } }));
    };

    // 1) Aplicar el tema desde localStorage en TODAS las páginas (incluye Identity)
    const stored = localStorage.getItem('bgClass'); // clave que ya usas en el selector
    if (stored) {
        applyTheme(stored);
    }

    // 2) Integrar con tu selector si existe en la página actual
    const colorSelector = document.querySelector("#colorSelector");
    if (colorSelector) {
        // Sincroniza selector con lo almacenado o con data-current si no había nada guardado
        const current = stored || colorSelector.dataset.current;
        if (current) {
            applyTheme(current);
            colorSelector.value = current;
        }

        // URL para guardar en servidor (ya la tienes en tu select con data-url)
        const url = colorSelector.dataset.url;

        colorSelector.addEventListener("change", () => {
            const selectedClass = colorSelector.value;

            // 2.a) Persistir en cliente y aplicar de inmediato
            localStorage.setItem('bgClass', selectedClass);
            applyTheme(selectedClass);

            // 2.b) Persistir en servidor (tu acción ChangeColor)
            fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ SelectedClass: selectedClass })
            })
                .then(r => r.json())
                .catch(err => console.error('Error al guardar el tema en servidor:', err));
        });
    }
});






//ELIMINAR UN USUARIO

document.addEventListener('DOMContentLoaded', () => {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const antiForgeryToken = tokenInput?.value;

    if (!antiForgeryToken) {
        console.error('No se encontró el token __RequestVerificationToken en el DOM.');
        return;
    }

    document.addEventListener('click', async (e) => {
        const link = e.target.closest('a.DeleteUser');
        if (!link) return;

        e.preventDefault();

        const url = link.getAttribute('href');
        const row = link.closest('tr');

        const result = await Swal.fire({
            title: '¿Estás seguro?',
            text: 'Esta acción eliminará el registro seleccionado.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6'
        });

        if (!result.isConfirmed) return;

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'RequestVerificationToken': antiForgeryToken,
                    'Accept': 'application/json'         
                }
               
            });

            if (!response.ok) {
                const errorText = await response.text().catch(() => '');
                console.error('DeleteUser error body:', errorText);
                Swal.fire({
                    title: `Error (HTTP ${response.status})`,
                    text: errorText || 'No se pudo eliminar.',
                    icon: 'error',
                    confirmButtonColor: '#3085d6'
                });
                return;
            }

            // --- Normalización de la respuesta ---


            let data = null;
            const contentType = response.headers.get('content-type') || '';

            if (response.status === 204 || response.status === 205) {
                // Sin contenido: tratamos como éxito
                data = {};
            } else if (contentType.includes('application/json')) {
                data = await response.json();
            } else {
                // No es JSON: lo leemos y lo mostramos en el texto de error
                const raw = await response.text().catch(() => '');
                data = { text: raw || 'Respuesta no JSON del servidor.' };
            }

            // ✅ Éxito inferido si el servidor responde OK aunque no haya 'success' en el cuerpo
            const success =
                (data?.success !== undefined)
                    ? !!data.success
                    : (response.ok && (response.status === 204 || contentType.includes('application/json')));

            // Defaults como pediste, basados en 'success' real/inferido
            const icon = data?.icon || (success ? 'success' : 'error');
            const title = data?.title || (success ? '¡Eliminado!' : 'Error');
            const text = data?.text || (success ? 'Operación completada.' : 'No se pudo eliminar.');

            // Quita la fila solo si hay éxito
            if (success) {
                row?.remove();
            }

            // Muestra el mensaje
            await Swal.fire({ title, text, icon, confirmButtonColor: '#3085d6' });



        } catch (err) {
            console.error(err);
            Swal.fire({
                title: 'Error',
                text: (err && err.message) || 'Error en la petición.',
                icon: 'error',
                confirmButtonColor: '#3085d6'
            });
        }
    });
});









//AÑADIR NUEVO CLUB

document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector("#AddNewCl");
    if (!form) return;
    const fields = ["ClubName", "ClubDescription", "AvailablePlace"];

    const validators = {


        ClubName: value => {
            if (!value) return 'El nombre del club es obligatorio';
            if (value.length > 50) return 'El nombre no puede superar los 50 caracteres';
            return '';
        },

        ClubDescription: value => {
            if (!value) return 'La descripción del club es obligatoria';
            if (value.length > 500) return 'La descripción no puede superar los 500 caracteres';
            return '';
        },

        AvailablePlace: value => {
            if (!value) return 'Especifica una cantidad de plazas';

            const num = parseInt(value, 10);
            if (num < 1 || num > 100) {
                return 'El número de plazas debe estar entre 1 y 100';
            }

            return '';
        }

    }

    const showError = (field, message) => {
        const input = form[field];
        const feedback = form.querySelector(`[data-valmsg-for="${field}"]`)
        if (message) {
            input.classList.add('is-invalid');
            input.classList.remove('is-valid');
            feedback.textContent = message;

        } else {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
            feedback.textContent = '';

        }
    };

    const validationField = field => {
        const allValues = Object.fromEntries(fields.map(f => [f, form[f].value.trim()]));
        const value = allValues[field];
        const error = field === 'Email' ? validators.Email() : validators[field](value, allValues);
        showError(field, error);

        return !error;
    };

    const validateForm = () => fields.map(validationField).every(Boolean);


    fields.forEach(field => {
        form[field].addEventListener('input', () => validationField(field));
    });


    form.addEventListener('submit', e => {
        if (!validateForm()) e.preventDefault();
    });


})











//ELIMINAR UN CLUB


document.addEventListener('DOMContentLoaded', () => {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    document.addEventListener('click', async (e) => {
        const link = e.target.closest('a.DeleteClub');
        if (!link) return;

        e.preventDefault();
        const url = link.getAttribute('href');
        const row = link.closest('tr');

        const confirm = await Swal.fire({
            title: '¿Estás seguro?',
            text: 'Esta acción eliminará el club y sus eventos asociados.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6'
        });
        if (!confirm.isConfirmed) return;

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                body: new URLSearchParams({ '__RequestVerificationToken': token })
            });

            const data = await response.json();
            const icon = data.icon || (data.success ? 'success' : 'error');
            const title = data.title || (data.success ? '¡Eliminado!' : 'Error');
            const text = data.text || 'Operación completada.';

            await Swal.fire({ icon, title, text });
            if (data.success) row?.remove();
        } catch {
            await Swal.fire({ icon: 'error', title: 'Error de red', text: 'No se pudo contactar con el servidor.' });
        }
    });
});















//MENSAJE DE YA INSCRITO

document.addEventListener("DOMContentLoaded", function () {
    var alertBox = document.getElementById("alertMessage");
    if (alertBox) {
        setTimeout(function () {
            alertBox.classList.remove("show");
            alertBox.classList.add("fade");
        }, 4000); // 4 segundos
    }
});








//GESTOR DE TAREAS



// Gestor de tareas del club (se activa solo si existe #club-tasks)
document.addEventListener('DOMContentLoaded', function () {
    const root = document.getElementById('club-tasks');
    if (!root) return; // en páginas sin gestor, no hace nada

    const clubId = root.getAttribute('data-club-id');
    const addUrl = root.getAttribute('data-add-url');
    const editUrl = root.getAttribute('data-edit-url');
    const completeUrl = root.getAttribute('data-complete-url');

    const addForm = root.querySelector('#addTaskForm');
    const descInput = root.querySelector('#taskDescription');
    const tbody = root.querySelector('#tasksTable tbody');

    // (opcional: diagnóstico mínimo)
    console.log('Gestor tareas listo', { clubId, addUrl });


    function post(url, data) {
        return fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams(data)
        }).then(async (r) => {
            if (!r.ok) {
                const txt = await r.text(); // útil para ver HTML de errores
                throw new Error('HTTP ' + r.status + ' - ' + txt.substring(0, 120));
            }
            return r.json();
        });
    }



    // Añadir tarea
    addForm.addEventListener('submit', function (e) {
        e.preventDefault();
        const description = (descInput.value || '').trim();
        if (!description) return;

        post(addUrl, { clubId, description })
            .then(res => {
                if (res && res.success && res.data) {
                    const tr = document.createElement('tr');
                    tr.setAttribute('data-id', res.data.id);
                    tr.innerHTML = `
          <td class="desc">
            <span class="text"></span>
            <input class="edit-input" type="text" style="display:none;" />
          </td>
          <td>
            <button class="btn-edit" type="button">Editar</button>
            <button class="btn-save" type="button" style="display:none;">Guardar</button>
            <button class="btn-cancel" type="button" style="display:none;">Cancelar</button>
            <button class="btn-complete" type="button">Completar</button>
          </td>`;
                    tr.querySelector('.text').textContent = res.data.description;
                    tr.querySelector('.edit-input').value = res.data.description;
                    tbody.prepend(tr);
                    descInput.value = '';
                } else {
                    alert((res?.title || 'Aviso') + ': ' + (res?.text || 'No se pudo añadir.'));
                }
            })
            .catch(err => {
                console.error(err);
                alert('No se pudo añadir la tarea. Revisa la URL de AddTask o el controlador.');
            });
    });


    // Delegación para Editar / Guardar / Cancelar / Completar
    tbody.addEventListener('click', function (e) {
        const btn = e.target;
        const tr = btn.closest('tr');
        if (!tr) return;

        const id = tr.getAttribute('data-id');
        const descCell = tr.querySelector('.desc');
        const span = descCell.querySelector('.text');
        const input = descCell.querySelector('.edit-input');
        const btnEdit = tr.querySelector('.btn-edit');
        const btnSave = tr.querySelector('.btn-save');
        const btnCancel = tr.querySelector('.btn-cancel');
        const btnComplete = tr.querySelector('.btn-complete');

        if (btn.classList.contains('btn-edit')) {
            input.value = span.textContent;
            span.style.display = 'none';
            input.style.display = '';
            btnEdit.style.display = 'none';
            btnSave.style.display = '';
            btnCancel.style.display = '';
            btnComplete.disabled = true;
            return;
        }

        if (btn.classList.contains('btn-save')) {
            const description = (input.value || '').trim();
            if (!description) { alert('La descripción no puede estar vacía.'); return; }
            post(editUrl, { id, description }).then(res => {
                if (res && res.success) {
                    span.textContent = description;
                    span.style.display = '';
                    input.style.display = 'none';
                    btnEdit.style.display = '';
                    btnSave.style.display = 'none';
                    btnCancel.style.display = 'none';
                    btnComplete.disabled = false;
                } else if (res) {
                    alert((res.title || 'Aviso') + ': ' + (res.text || 'No se pudo editar.'));
                }
            });
            return;
        }

        if (btn.classList.contains('btn-cancel')) {
            input.value = span.textContent;
            span.style.display = '';
            input.style.display = 'none';
            btnEdit.style.display = '';
            btnSave.style.display = 'none';
            btnCancel.style.display = 'none';
            btnComplete.disabled = false;
            return;
        }

        if (btn.classList.contains('btn-complete')) {
            post(completeUrl, { id }).then(res => {
                if (res && res.success) {
                    tr.remove();
                } else if (res) {
                    alert((res.title || 'Aviso') + ': ' + (res.text || 'No se pudo completar.'));
                }
            });
            return;
        }
    });
});




//LIMPIEZA DE CUENTAS PERDIDAS
//Función para buscar y eliminar todas aquellas cuentas que no se encuentran con un rol asignado o esten en pending user.

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('clean-orphans-form');
    const btn = document.getElementById('btnCleanOrphans');
    if (!form || !btn) return;

    btn.addEventListener('click', async (e) => {
        e.preventDefault(); // por si acaso

        const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const res = await fetch(form.action, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token || '',
                'X-Requested-With': 'XMLHttpRequest',
                'Accept': 'application/json'
            }
        });

        const isJson = (res.headers.get('content-type') || '').includes('application/json');
        const data = isJson ? await res.json() : { ok: res.ok, title: res.ok ? 'Limpieza realizada' : 'Error', text: isJson ? '' : 'Respuesta no válida.' };

        const success = !!data.ok;
        const icon = data.icon || (success ? 'success' : 'error');
        const title = data.title || (success ? 'Limpieza realizada' : 'Error');
        const text = data.text || (success ? 'Operación completada.' : 'No se pudo ejecutar.');

        await Swal.fire({ title, text, icon, confirmButtonColor: '#3085d6' });
    });
});






