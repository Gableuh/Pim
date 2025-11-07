// Alternar views
const navButtons = document.querySelectorAll('.nav-item');
const views = {
    recents: document.getElementById('view-recents'),
    abrir: document.getElementById('view-abrir'),
    meus: document.getElementById('view-meus')
};
navButtons.forEach(btn => {
    btn.addEventListener('click', () => {
        navButtons.forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        const view = btn.dataset.view;
        Object.keys(views).forEach(k => { if (views[k]) views[k].style.display = (k === view) ? '' : 'none'; });
    });
});

// Sidebar toggle mobile
const toggleSidebarBtn = document.getElementById('toggleSidebar');
const sidebar = document.getElementById('sidebar');
toggleSidebarBtn.addEventListener('click', () => {
    sidebar.classList.toggle('open');
});
