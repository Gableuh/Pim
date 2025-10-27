document.addEventListener('DOMContentLoaded', () => {
  const toggle = document.getElementById('toggleTheme');
  const themeLink = document.getElementById('theme-css');

  // Mantém a escolha anterior do usuário
  if (localStorage.getItem('theme') === 'dark') {
    toggle.checked = true;
    if(themeLink.href.includes('dashboard-tecnico')) {
      themeLink.href = 'css/dashboard-tecnico-cards-dark.css';
    } else {
      themeLink.href = 'css/dashboard-cliente-dark.css';
    }
  }

  toggle.addEventListener('change', () => {
    if (toggle.checked) {
      if(themeLink.href.includes('dashboard-tecnico')) {
        themeLink.href = 'css/dashboard-tecnico-cards-dark.css';
      } else {
        themeLink.href = 'css/dashboard-cliente-dark.css';
      }
      localStorage.setItem('theme', 'dark');
    } else {
      if(themeLink.href.includes('dashboard-tecnico')) {
        themeLink.href = 'css/dashboard-tecnico-cards-light.css';
      } else {
        themeLink.href = 'css/dashboard-cliente-light.css';
      }
      localStorage.setItem('theme', 'light');
    }
  });
});
