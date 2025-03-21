import { createPlayer } from "../../services/apiService.js";
import BaseModal from "./BaseModal.js";

const CreatePlayerModal = (onSubmit) => {
  const fields = [
    { id: "name", label: "Name", placeholder: "Enter name", required: true },
    {
      id: "username",
      label: "Username",
      placeholder: "Enter username",
      required: true,
    },
    {
      id: "profileImageUrl",
      label: "Profile Image URL",
      placeholder: "Enter image URL",
    },
  ];

  BaseModal({
    id: "create-player-modal",
    title: "Create New Player",
    fields,
    submitText: "Create Player",
    onSubmit,
    apiCall: (formData) => createPlayer(formData),
  });
};

export default CreatePlayerModal;
