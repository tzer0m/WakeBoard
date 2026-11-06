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
    }
  } catch (error) {
    console.error(error);
  }
}

refreshStatus();
setInterval(refreshStatus, Number(document.querySelector('table').dataset.refresh));
