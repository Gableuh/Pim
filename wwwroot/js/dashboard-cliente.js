document.addEventListener('DOMContentLoaded', () => {
  const themeToggle = document.getElementById('toggleTheme');
  const themeLink = document.getElementById('theme-css');
  const tbody = document.getElementById('client-tickets');

  // Dark mode
  if (localStorage.getItem('clientTheme') === 'dark') {
    themeLink.href = 'css/dashboard-cliente-dark.css';
    themeToggle.checked = true;
  }
  themeToggle.addEventListener('change', () => {
    if (themeToggle.checked) {
      themeLink.href = 'css/dashboard-cliente-dark.css';
      localStorage.setItem('clientTheme', 'dark');
    } else {
      themeLink.href = 'css/dashboard-cliente-light.css';
      localStorage.setItem('clientTheme', 'light');
    }
  });

  // Carregar chamados
  async function carregarChamados() {
    try {
      const userId = 1; // ID fixo
      const resp = await fetch(`http://localhost:5278/api/chamados/usuario/${userId}`);
      if (!resp.ok) throw new Error(`Erro ao buscar chamados: ${resp.status}`);
      const chamados = await resp.json();

      tbody.innerHTML = '';
      if (chamados.length === 0) {
        tbody.innerHTML = `<tr><td colspan="4">Nenhum chamado encontrado.</td></tr>`;
        return;
      }

      chamados.forEach(t => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
          <td>${t.ticketId}</td>
          <td>${t.title}</td>
          <td>${t.status}</td>
          <td>${t.createdAt ? new Date(t.createdAt).toLocaleDateString() : ''}</td>
        `;
        tbody.appendChild(tr);
      });
    } catch (err) {
      console.error(err);
      tbody.innerHTML = `<tr><td colspan="4">Não foi possível carregar os chamados.</td></tr>`;
    }
  }

  carregarChamados();

  // Modal
  const modal = document.getElementById('ticket-modal');
  const openBtn = document.getElementById('new-ticket');
  const closeBtn = document.getElementById('close-modal');
  const cancelBtn = document.getElementById('cancel-ticket');
  const submitBtn = document.getElementById('submit-ticket');

  openBtn.addEventListener('click', () => modal.style.display = 'flex');
  closeBtn.addEventListener('click', () => modal.style.display = 'none');
  cancelBtn.addEventListener('click', () => modal.style.display = 'none');

  // Salvar chamado
  submitBtn.addEventListener('click', async () => {
    const novoChamado = {
      title: document.getElementById('ticket-title').value.trim(),
      description: document.getElementById('ticket-description').value.trim(),
      categoryId: parseInt(document.getElementById('ticket-category').value),
      priority: parseInt(document.getElementById('ticket-priority').value),
      createdBy: 1,
      status: "Aberto"
    };

    if (!novoChamado.title || isNaN(novoChamado.categoryId) || isNaN(novoChamado.priority)) {
      alert('Preencha todos os campos corretamente.');
      return;
    }

    try {
      const resp = await fetch('http://localhost:5278/api/chamados', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(novoChamado)
      });

      if (!resp.ok) {
        const errMsg = await resp.json().catch(() => ({}));
        throw new Error(errMsg.message || resp.status);
      }

      const result = await resp.json();
      alert(result.message || 'Chamado criado com sucesso!');
      modal.style.display = 'none';

      // Limpar campos
      document.getElementById('ticket-title').value = '';
      document.getElementById('ticket-description').value = '';
      document.getElementById('ticket-category').value = '';
      document.getElementById('ticket-priority').value = '';

      // Atualizar tabela
      carregarChamados();
    } catch (err) {
      console.error(err);
      alert(`Erro ao criar chamado: ${err}`);
    }
  });
});
