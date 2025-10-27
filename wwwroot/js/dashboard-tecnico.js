document.addEventListener('DOMContentLoaded', () => {
  const toggle = document.getElementById('toggleTheme');
  const themeLink = document.getElementById('theme-css');

  // Mantém tema salvo
  if(localStorage.getItem('techTheme')==='dark'){
    themeLink.href='css/dashboard-tecnico-dark.css';
    toggle.checked=true;
  }

  toggle.addEventListener('change', ()=>{
    if(toggle.checked){
      themeLink.href='css/dashboard-tecnico-dark.css';
      localStorage.setItem('techTheme','dark');
    } else {
      themeLink.href='css/dashboard-tecnico-light.css';
      localStorage.setItem('techTheme','light');
    }
  });

  // Função para buscar chamados da API
  async function fetchTickets() {
    try {
      const resp = await fetch('http://localhost:5278/api/chamados');
      if(!resp.ok) throw new Error('Falha ao carregar chamados');
      const tickets = await resp.json();
      const tbody = document.getElementById('tickets-table').querySelector('tbody');
      tbody.innerHTML = ''; // Limpa tabela antes de preencher

      tickets.forEach(t => {
        const tr = document.createElement('tr');
        const prioridadeClass = {
          'Urgente':'chamado-urgente',
          'Alta':'chamado-alta',
          'Média':'chamado-media',
          'Baixa':'chamado-baixa'
        }[t.prioridade] || '';

        tr.innerHTML = `
          <td>${t.id}</td>
          <td>${t.cliente}</td>
          <td><span class="${prioridadeClass}">${t.prioridade}</span></td>
          <td>${t.status}</td>
          <td><button>Assumir</button></td>
        `;
        tbody.appendChild(tr);
      });

    } catch(err) {
      console.error(err);
      alert('Erro ao carregar chamados. Veja o console.');
    }
  }

  fetchTickets();
});
