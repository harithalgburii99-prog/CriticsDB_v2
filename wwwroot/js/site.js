// ===== CriticsDB Site JS =====

document.addEventListener('DOMContentLoaded', () => {
    initTheme();
    initUserMenu();
    initToastAutoDismiss();
    initNavHighlight();
    initAnimations();
});

// ── Theme Toggle ──
function initTheme() {
    const toggle = document.getElementById('themeToggle');
    const icon = document.getElementById('themeIcon');
    const html = document.documentElement;

    const saved = localStorage.getItem('theme') || 'dark';
    html.setAttribute('data-theme', saved);
    updateThemeIcon(saved, icon);

    toggle?.addEventListener('click', () => {
        const current = html.getAttribute('data-theme');
        const next = current === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-theme', next);
        localStorage.setItem('theme', next);
        updateThemeIcon(next, icon);
    });
}

function updateThemeIcon(theme, icon) {
    if (!icon) return;
    icon.className = theme === 'dark' ? 'fas fa-moon' : 'fas fa-sun';
}

// ── User Dropdown ──
function initUserMenu() {
    const btn = document.getElementById('userMenuBtn');
    const dropdown = document.getElementById('userDropdown');
    if (!btn || !dropdown) return;

    btn.addEventListener('click', (e) => {
        e.stopPropagation();
        dropdown.classList.toggle('open');
    });

    document.addEventListener('click', (e) => {
        if (!btn.contains(e.target) && !dropdown.contains(e.target)) {
            dropdown.classList.remove('open');
        }
    });
}

// ── Toast Auto-Dismiss ──
function initToastAutoDismiss() {
    document.querySelectorAll('.toast').forEach(toast => {
        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(20px)';
            toast.style.transition = 'all .4s ease';
            setTimeout(() => toast.remove(), 400);
        }, 4000);
    });
}

// ── Active Nav ──
function initNavHighlight() {
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-links a').forEach(a => {
        const href = a.getAttribute('href')?.toLowerCase();
        if (href && href !== '/' && path.startsWith(href)) {
            a.style.color = 'var(--text-primary)';
            a.style.background = 'var(--bg-elevated)';
        }
    });
}

// ── Scroll Animations ──
function initAnimations() {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1 });

    document.querySelectorAll('.movie-card, .review-card, .admin-stat-card').forEach(el => {
        el.classList.add('fade-in');
        observer.observe(el);
    });
}

// ── Rating bars animate on load ──
window.addEventListener('load', () => {
    document.querySelectorAll('.bar-fill').forEach(bar => {
        const w = bar.style.width;
        bar.style.width = '0';
        requestAnimationFrame(() => {
            setTimeout(() => { bar.style.width = w; }, 100);
        });
    });
});

// ── Add fade-in CSS ──
const style = document.createElement('style');
style.textContent = `
    .fade-in { opacity: 0; transform: translateY(12px); transition: opacity .4s ease, transform .4s ease; }
    .fade-in.visible { opacity: 1; transform: translateY(0); }
`;
document.head.appendChild(style);
