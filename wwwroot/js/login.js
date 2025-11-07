document.addEventListener('DOMContentLoaded', () => {
  // --- TOGGLE DE TEMA ---
  const toggleTheme = document.getElementById('toggleTheme');
  const themeLink = document.getElementById('theme-css');

  // Mantém tema salvo
  if(localStorage.getItem('theme') === 'dark'){
    themeLink.href = 'css/styles-dark.css';
    toggleTheme.checked = true;
  }

  toggleTheme.addEventListener('change', () => {
    if(toggleTheme.checked){
      themeLink.href = 'css/styles-dark.css';
      localStorage.setItem('theme','dark');
    } else {
      themeLink.href = 'css/styles.css';document.addEventListener('DOMContentLoaded', () => {
  // --- TOGGLE DE TEMA ---
  const toggleTheme = document.getElementById('toggleTheme');
  const themeLink = document.getElementById('theme-css');

  // Mantém tema salvo
  if(localStorage.getItem('theme') === 'dark'){
    themeLink.href = 'css/styles-dark.css';
    toggleTheme.checked = true;
  }

  toggleTheme.addEventListener('change', () => {
    if(toggleTheme.checked){
      themeLink.href = 'css/styles-dark.css';
      localStorage.setItem('theme','dark');
    } else {
      themeLink.href = 'css/styles.css';
      localStorage.setItem('theme','light');
    }
  });

  // --- SHOW/HIDE SENHA ---
  const form = document.getElementById('loginForm');
  const msg = document.getElementById('formMessage');
  const togglePassword = document.getElementById('togglePassword');
  const password = document.getElementById('password');

  togglePassword.addEventListener('click', () => {
    if(password.type === 'password'){
      password.type = 'text';
      togglePassword.textContent = '🙈';
    } else {
      password.type = 'password';
      togglePassword.textContent = '👁️';
    }
  });

  // --- SUBMIT LOGIN COM API ---
  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    msg.textContent = '';

    const email = form.email.value.trim();
    const pwd = password.value;
    const role = form.role.value;

    if(!email || !pwd){ msg.textContent='Preencha e-mail e senha.'; return; }
    const re = /\S+@\S+\.\S+/;
    if(!re.test(email)){ msg.textContent='E-mail inválido.'; return; }

    msg.style.color = '#374151';
    msg.textContent = 'Entrando...';

    try {
      const resp = await fetch('http://localhost:5278/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password: pwd, role })
      });

      if(!resp.ok) throw new Error('Autenticação falhou');
      const data = await resp.json();
      console.log('Resposta API:', data);

      await new Promise(r => setTimeout(r, 900));
      msg.style.color = '#064e3b';
      msg.textContent = 'Login efetuado. Redirecionando...';

      setTimeout(() => {
        if(role === 'tecnico') window.location.href = 'dashboard-tecnico.html';
        else window.location.href = 'dashboard-cliente.html';
      }, 600);

    } catch(err) {
      console.error(err);
      msg.style.color = '#b91c1c';
      msg.textContent = 'Erro ao conectar com o servidor.';
    }
  });
});

      localStorage.setItem('theme','light');
    }
  });

  // --- SHOW/HIDE SENHA ---
  const form = document.getElementById('loginForm');
  const msg = document.getElementById('formMessage');
  const togglePassword = document.getElementById('togglePassword');
  const password = document.getElementById('password');

  togglePassword.addEventListener('click', () => {
    if(password.type === 'password'){
      password.type = 'text';
      togglePassword.textContent = '🙈';
    } else {
      password.type = 'password';
      togglePassword.textContent = '👁️';
    }
  });

  // --- SUBMIT LOGIN COM API ---
  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    msg.textContent = '';

    const email = form.email.value.trim();
    const pwd = password.value;
    const role = form.role.value;

    if(!email || !pwd){ msg.textContent='Preencha e-mail e senha.'; return; }
    const re = /\S+@\S+\.\S+/;
    if(!re.test(email)){ msg.textContent='E-mail inválido.'; return; }

    msg.style.color = '#374151';
    msg.textContent = 'Entrando...';

    try {
      const resp = await fetch('http://localhost:5278/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password: pwd, role })
      });

      if(!resp.ok) throw new Error('Autenticação falhou');
      const data = await resp.json();
      console.log('Resposta API:', data);

      await new Promise(r => setTimeout(r, 900));
      msg.style.color = '#064e3b';
      msg.textContent = 'Login efetuado. Redirecionando...';

      setTimeout(() => {
        if(role === 'tecnico') window.location.href = 'dashboard-tecnico.html';
        else window.location.href = 'dashboard-cliente.html';
      }, 600);

    } catch(err) {
      console.error(err);
      msg.style.color = '#b91c1c';
      msg.textContent = 'Erro ao conectar com o servidor.';
    }
  });
});
