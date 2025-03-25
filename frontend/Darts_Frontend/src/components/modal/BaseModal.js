const BaseModal = ({
  id,
  title,
  fields,
  submitText,
  onSubmit,
  initialData = {},
  apiCall,
}) => {
  const modal = document.createElement("div");
  modal.id = id;
  modal.className = "modal";
  modal.style.display = "none";

  const fieldsHtml = fields
    .map((field) => {
      const value = initialData[field.id] || "";
      return `
        <label for="${field.id}">${field.label}:</label>
        <input type="${field.type || "text"}" id="${
        field.id
      }" value="${value}" placeholder="${field.placeholder || ""}" />
      `;
    })
    .join("");

  modal.innerHTML = `
    <div class="modal-content">
      <span id="${id}-close" class="modal-close">×</span>
      <h2>${title}</h2>
      <div id="${id}-error" class="error-message" style="display: none;"></div>
      <div class="modal-form">
        ${fieldsHtml}
        <button id="${id}-submit" class="modal-submit-btn">${submitText}</button>
      </div>
    </div>
  `;

  document.body.appendChild(modal);

  const errorMessage = document.getElementById(`${id}-error`);

  const closeModal = () => {
    modal.style.display = "none";
    modal.remove();
  };

  document.getElementById(`${id}-close`).addEventListener("click", closeModal);

  document
    .getElementById(`${id}-submit`)
    .addEventListener("click", async () => {
      errorMessage.style.display = "none";

      const formData = fields.reduce((data, field) => {
        const value = document.getElementById(field.id).value.trim();
        data[field.id] = field.type === "text" && !value ? null : value;
        return data;
      }, {});

      const requiredFields = fields.filter((f) => f.required).map((f) => f.id);
      const missingFields = requiredFields.filter((id) => !formData[id]);
      if (missingFields.length > 0) {
        errorMessage.textContent = `${missingFields
          .map((id) => fields.find((f) => f.id === id).label)
          .join(" and ")} ${missingFields.length > 1 ? "are" : "is"} required.`;
        errorMessage.style.display = "block";
        return;
      }

      try {
        const result = await apiCall(formData);
        onSubmit(result);
        closeModal();
      } catch (error) {
        console.error(`Error in ${title.toLowerCase()}:`, error);
        let errorText = `Failed to ${title.toLowerCase()}. Please try again.`;
        if (error.message) {
          errorText = error.message;
        } else if (error.response && error.response.data) {
          errorText =
            error.response.data.detail ||
            error.response.data.message ||
            errorText;
        }

        errorMessage.textContent = errorText;
        errorMessage.style.display = "block";
      }
    });

  modal.style.display = "block";
};

export default BaseModal;
