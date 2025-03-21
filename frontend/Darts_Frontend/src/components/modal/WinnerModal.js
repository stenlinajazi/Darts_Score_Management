const WinnerModal = (title, message, onClose) => {
  const modal = document.createElement("div");
  modal.id = "winner-modal";
  modal.className = "modal";
  modal.innerHTML = `
    <div class="modal-content">
      <span id="modal-close" class="modal-close">×</span>
      <h2 id="modal-title">${title}</h2>
      <p id="modal-message">${message}</p>
      <button id="modal-ok" class="btn btn-primary">OK</button>
    </div>
  `;

  document.body.appendChild(modal);

  const closeModal = () => {
    modal.style.display = "none";
    modal.remove();
    if (onClose) onClose();
  };

  document.getElementById("modal-close").addEventListener("click", closeModal);
  document.getElementById("modal-ok").addEventListener("click", closeModal);

  modal.style.display = "block";
};

export default WinnerModal;
