import { updatePlayer } from "../../services/apiService.js";
import BaseModal from "./BaseModal.js";

const UpdatePlayerModal = (player, onSubmit) => {
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
    id: "update-player-modal",
    title: "Update Player",
    fields,
    submitText: "Update Player",
    onSubmit,
    initialData: {
      name: player.name,
      username: player.username,
      profileImageUrl: player.profileImageUrl || "",
    },
    apiCall: (formData) => updatePlayer(player.id, formData),
  });
};

export default UpdatePlayerModal;
