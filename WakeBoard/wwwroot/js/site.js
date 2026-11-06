async function refreshStatus() {
  try {
    const response = await fetch(location.pathname + '?handler=Status');
    const statuses = await response.json();
    for (const status of statuses) {
      const row = document.querySelector('tr[data-name="' + CSS.escape(status.name) + '"]');
      if (!row) { continue; }
      const badge = row.querySelector('.badge');
      badge.className = 'badge ' + (status.online ? 'online' : 'offline');
      badge.textContent = status.online ? 'Online' : 'Offline';
      row.querySelectorAll('.wake').forEach(button => { button.disabled = status.online; });
      row.querySelectorAll('.power').forEach(button => { button.disabled = !status.online; });
    }
  } catch (error) {
    console.error(error);
  }
}

document.addEventListener('click', event => {
  const target = event.target.closest('[data-confirm]');
  if (target && !confirm(target.dataset.confirm)) { event.preventDefault(); }
});

document.addEventListener('submit', async event => {
  event.preventDefault();
  const button = event.submitter;
  button.disabled = true;
  const message = document.getElementById('message');
  try {
    const response = await fetch(button.formAction, { method: 'POST', body: new FormData(event.target), headers: { 'X-Requested-With': 'fetch' } });
    const result = await response.json();
    message.textContent = result.message;
  } catch (error) {
    console.error(error);
    message.textContent = 'The request failed.';
  }
  message.hidden = false;
  refreshStatus();
});

refreshStatus();
setInterval(refreshStatus, Number(document.querySelector('table').dataset.refresh));
