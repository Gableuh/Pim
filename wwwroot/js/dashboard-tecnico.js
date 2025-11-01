document.addEventListener('DOMContentLoaded', () => {

    // =============================
    // Modo dark/light já existente
    // =============================
    const toggle = document.getElementById('toggleTheme');
    const themeLink = document.getElementById('theme-css');

    if (toggle && themeLink) {
        if (localStorage.getItem('techTheme') === 'dark') {
            themeLink.href = 'css/dashboard-tecnico-dark.css';
            toggle.checked = true;
        }

        toggle.addEventListener('change', () => {
            if (toggle.checked) {
                themeLink.href = 'css/dashboard-tecnico-dark.css';
                localStorage.setItem('techTheme', 'dark');
            } else {
                themeLink.href = 'css/dashboard-tecnico-light.css';
                localStorage.setItem('techTheme', 'light');
            }
        });
    }

    // =====================================
    // Sidebar recolhível e troca de views
    // =====================================
    const navButtons = document.querySelectorAll('.nav-item');
    const views = {
        recents: document.getElementById('view-recents'),
        meus: document.getElementById('view-meus')
    };

    navButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            navButtons.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            const view = btn.dataset.view;
            Object.keys(views).forEach(k => {
                if (views[k]) views[k].style.display = (k === view) ? '' : 'none';
            });
        });
    });

    // =====================================
    // Toggle sidebar no mobile
    // =====================================
    const sidebar = document.querySelector('.sidebar');
    const hamburger = document.createElement('button');
    hamburger.textContent = '☰';
    hamburger.style.position = 'fixed';
    hamburger.style.top = '16px';
    hamburger.style.left = '16px';
    hamburger.style.zIndex = '50';
    hamburger.style.fontSize = '20px';
    hamburger.style.background = 'transparent';
    hamburger.style.border = 'none';
    hamburger.style.cursor = 'pointer';
    document.body.appendChild(hamburger);

    hamburger.addEventListener('click', () => {
        sidebar.classList.toggle('open');
    });

    // =====================================
    // Logout
    // =====================================
    document.getElementById('logoutBtn')?.addEventListener('click', () => {
        window.location.href = '/Dashboard/Logout';
    });
    document.querySelector('.top-buttons .fixed')?.addEventListener('click', () => {
        window.location.href = '/Dashboard/Logout';
    });

    // =====================================
    // Busca simples
    // =====================================
    const searchBtn = document.getElementById('searchBtn');
    if (searchBtn) {
        searchBtn.addEventListener('click', () => {
            const id = document.getElementById('searchId').value.toLowerCase();
            const assunto = document.getElementById('searchAssunto').value.toLowerCase();
            const data = document.getElementById('searchData').value;
            const setor = document.getElementById('searchSetor').value.toLowerCase();
            const prioridade = document.getElementById('searchPrioridade').value.toLowerCase();
            const colaborador = document.getElementById('searchColaborador').value.toLowerCase();
            const rows = document.querySelectorAll('#chamadosTable tbody tr');

            rows.forEach(row => {
                const cells = row.querySelectorAll('td');
                const match = (!id || cells[0].textContent.toLowerCase().includes(id)) &&
                    (!assunto || cells[1].textContent.toLowerCase().includes(assunto)) &&
                    (!data || cells[2].textContent.includes(data)) &&
                    (!setor || cells[3].textContent.toLowerCase().includes(setor)) &&
                    (!prioridade || cells[4].textContent.toLowerCase().includes(prioridade)) &&
                    (!colaborador || cells[5].textContent.toLowerCase().includes(colaborador));
                row.style.display = match ? '' : 'none';
            });
        });
    }

});
