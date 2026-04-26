// SmartCare — Site JS

// Auto-dismiss alerts after 5s
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.alert.alert-success, .alert.alert-danger').forEach(function (el) {
        if (!el.querySelector('.btn-close')) return;
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(el);
            if (bsAlert) bsAlert.close();
        }, 5000);
    });

    // Active nav link highlight
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar .nav-link').forEach(function (link) {
        const href = link.getAttribute('href');
        if (href && path.startsWith(href.toLowerCase()) && href !== '/') {
            link.classList.add('active');
        }
    });
});

// Confirm before delete/destructive actions
document.querySelectorAll('[data-confirm]').forEach(function (el) {
    el.addEventListener('click', function (e) {
        if (!confirm(this.dataset.confirm)) e.preventDefault();
    });
});
