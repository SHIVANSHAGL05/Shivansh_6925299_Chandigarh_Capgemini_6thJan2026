/**
 * students.js – Handles all CRUD interactions for the Student Management page.
 * Communicates with StudentsController via AJAX/fetch.
 */

"use strict";

// ── helpers ──────────────────────────────────────────────────────────────────

const studentModal  = new bootstrap.Modal(document.getElementById('studentModal'));
const deleteModal   = new bootstrap.Modal(document.getElementById('deleteModal'));

function getToken() {
    return document.querySelector('#studentForm input[name="__RequestVerificationToken"]').value;
}

function showToast(message, type = 'success') {
    const id   = 'toast-' + Date.now();
    const icon = type === 'success' ? 'check-circle-fill' : 'x-circle-fill';
    const html = `
        <div id="${id}" class="toast align-items-center text-bg-${type} border-0 show shadow" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi bi-${icon} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>`;
    const container = document.getElementById('toastContainer');
    container.insertAdjacentHTML('beforeend', html);
    setTimeout(() => document.getElementById(id)?.remove(), 4000);
}

function clearFormErrors() {
    ['Name','Email','Course','JoiningDate'].forEach(field => {
        const input = document.getElementById('input' + field);
        if (input) {
            input.classList.remove('is-invalid', 'is-valid');
        }
        const errEl = document.getElementById(field.charAt(0).toLowerCase() + field.slice(1) + 'Error');
        if (errEl) errEl.textContent = '';
    });
    document.getElementById('formErrorSummary').classList.add('d-none');
}

function applyServerErrors(errors) {
    const summary = [];
    for (const [key, messages] of Object.entries(errors)) {
        const shortKey = key.split('.').pop();
        const input    = document.getElementById('input' + shortKey);
        const errEl    = document.getElementById(shortKey.charAt(0).toLowerCase() + shortKey.slice(1) + 'Error');
        if (input)  input.classList.add('is-invalid');
        if (errEl)  errEl.textContent = messages.join(' ');
        summary.push(...messages);
    }
    if (summary.length) {
        const el = document.getElementById('formErrorSummary');
        el.textContent = summary.join(' | ');
        el.classList.remove('d-none');
    }
}

// ── ADD NEW button ────────────────────────────────────────────────────────────

document.getElementById('btnAddNew').addEventListener('click', () => {
    clearFormErrors();
    document.getElementById('studentId').value = 0;
    document.getElementById('studentForm').reset();
    document.getElementById('modalTitleText').textContent = 'Add New Student';
    document.querySelector('#studentModal .modal-header .bi').className = 'bi bi-person-plus-fill me-2';
    studentModal.show();
});

// ── EDIT buttons ──────────────────────────────────────────────────────────────

document.addEventListener('click', async e => {
    const btn = e.target.closest('.btn-edit');
    if (!btn) return;

    const id = btn.dataset.id;
    try {
        const res  = await fetch(`/Students/Edit/${id}`);
        if (!res.ok) throw new Error('Student not found');
        const data = await res.json();

        clearFormErrors();
        document.getElementById('studentId').value        = data.id;
        document.getElementById('inputName').value        = data.name;
        document.getElementById('inputEmail').value       = data.email;
        document.getElementById('inputCourse').value      = data.course;
        document.getElementById('inputJoiningDate').value = data.joiningDate?.substring(0, 10);
        document.getElementById('modalTitleText').textContent = 'Edit Student';
        studentModal.show();
    } catch (err) {
        showToast('Could not load student data.', 'danger');
    }
});

// ── SAVE (Create / Update) ────────────────────────────────────────────────────

document.getElementById('btnSaveStudent').addEventListener('click', async () => {
    const form = document.getElementById('studentForm');

    // Client-side validation via Bootstrap
    if (!form.checkValidity()) {
        form.querySelectorAll(':invalid').forEach(el => el.classList.add('is-invalid'));
        form.querySelectorAll(':valid').forEach(el => el.classList.remove('is-invalid'));
        return;
    }

    clearFormErrors();

    const id         = parseInt(document.getElementById('studentId').value);
    const isEdit     = id > 0;
    const url        = isEdit ? '/Students/Edit' : '/Students/Create';
    const formData   = new FormData(form);

    try {
        const res  = await fetch(url, {
            method: 'POST',
            headers: { 'RequestVerificationToken': getToken() },
            body: formData
        });
        const data = await res.json();

        if (data.success) {
            studentModal.hide();
            showToast(isEdit ? 'Student updated successfully.' : 'Student added successfully.');
            setTimeout(() => location.reload(), 900);
        } else {
            applyServerErrors(data.errors);
        }
    } catch {
        showToast('An unexpected error occurred.', 'danger');
    }
});

// ── DELETE ────────────────────────────────────────────────────────────────────

let pendingDeleteId = null;

document.addEventListener('click', e => {
    const btn = e.target.closest('.btn-delete');
    if (!btn) return;
    pendingDeleteId = btn.dataset.id;
    document.getElementById('deleteStudentName').textContent = btn.dataset.name;
    deleteModal.show();
});

document.getElementById('btnConfirmDelete').addEventListener('click', async () => {
    if (!pendingDeleteId) return;

    const token = document.querySelector('#deleteForm input[name="__RequestVerificationToken"]').value;
    try {
        const res  = await fetch(`/Students/Delete/${pendingDeleteId}`, {
            method: 'POST',
            headers: { 'RequestVerificationToken': token }
        });
        const data = await res.json();

        if (data.success) {
            deleteModal.hide();
            showToast('Student deleted successfully.', 'danger');
            setTimeout(() => location.reload(), 900);
        } else {
            showToast('Could not delete student.', 'danger');
        }
    } catch {
        showToast('An unexpected error occurred.', 'danger');
    }
});

// ── CLIENT-SIDE SEARCH / FILTER ───────────────────────────────────────────────

function filterTable() {
    const query  = document.getElementById('searchInput').value.toLowerCase();
    const course = document.getElementById('courseFilter').value.toLowerCase();
    const rows   = document.querySelectorAll('#studentsTable tbody tr');
    let   visible = 0;

    rows.forEach(row => {
        const matchText   = !query  || row.dataset.name.includes(query)  || row.dataset.email.includes(query) || row.dataset.course.toLowerCase().includes(query);
        const matchCourse = !course || row.dataset.course.toLowerCase() === course;
        const show        = matchText && matchCourse;
        row.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    document.getElementById('rowCount').textContent = visible + ' record' + (visible !== 1 ? 's' : '');
}

document.getElementById('searchInput').addEventListener('input',  filterTable);
document.getElementById('courseFilter').addEventListener('change', filterTable);

// Bootstrap inline validation — mark valid fields on blur
document.getElementById('studentForm').querySelectorAll('.form-control').forEach(input => {
    input.addEventListener('blur', () => {
        if (input.value) {
            input.classList.toggle('is-invalid', !input.checkValidity());
            input.classList.toggle('is-valid',   input.checkValidity());
        }
    });
});
