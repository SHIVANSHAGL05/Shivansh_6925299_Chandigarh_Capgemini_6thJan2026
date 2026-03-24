"use strict";

// ── Navbar scroll class ──────────────────────────────────────────
const nav = document.getElementById('mainNav');
if (nav) {
    window.addEventListener('scroll', () => {
        nav.classList.toggle('scrolled', window.scrollY > 40);
    }, { passive: true });
}

// ── Live client-side search on /Products page ────────────────────
const liveSearch = document.getElementById('liveSearch');
const grid       = document.getElementById('productGrid');

if (liveSearch && grid) {
    liveSearch.addEventListener('input', () => {
        const q = liveSearch.value.toLowerCase().trim();
        let visible = 0;

        grid.querySelectorAll('.product-grid-item').forEach(item => {
            const match = !q ||
                item.dataset.name.includes(q) ||
                item.dataset.category.includes(q);
            item.style.display = match ? '' : 'none';
            if (match) visible++;
        });

        // Update count indicator
        const subtitle = document.querySelector('.page-subtitle');
        if (subtitle) {
            subtitle.textContent = visible + ' product' + (visible !== 1 ? 's' : '') + ' found'
                + (q ? ` for "${liveSearch.value}"` : '');
        }
    });
}

// ── Animate cards on load ────────────────────────────────────────
document.querySelectorAll('.product-card').forEach((card, i) => {
    card.style.opacity = '0';
    card.style.transform = 'translateY(20px)';
    card.style.transition = `opacity 0.4s ease ${i * 0.06}s, transform 0.4s ease ${i * 0.06}s`;
    requestAnimationFrame(() => {
        card.style.opacity = '1';
        card.style.transform = 'translateY(0)';
    });
});
