import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";
import { assignUser, removeUser, clearUsers } from "../../../api/api";

const PlanProcedureItem = ({ procedure, users, assignedUserIds = [], planId }) => {
  const [selectedUsers, setSelectedUsers] = useState([]);

  // 🧠 Initialize selectedUsers from assignedUserIds
  useEffect(() => {
    const selected = users.filter((u) => assignedUserIds.includes(u.value));
    setSelectedUsers(selected);
  }, [assignedUserIds, users]);

  const handleAssignUserToProcedure = async (newSelected) => {
    const newUserIds = newSelected.map((u) => u.value);
    const oldUserIds = selectedUsers.map((u) => u.value);

    const toAdd = newUserIds.filter((id) => !oldUserIds.includes(id));

    // If the user is unselecting all users, we can use the clearUsers endpoint
    if (newUserIds.length === 0 && oldUserIds.length > 0) {
      try {
        await clearUsers({ planId: parseInt(planId), procedureId: procedure.procedureId });
        setSelectedUsers([]);
      } catch (err) {
        alert("Failed to clear users");
      }
      return;
    }

    const toRemove = oldUserIds.filter((id) => !newUserIds.includes(id));

    try {
      for (const userId of toAdd) {
        await assignUser({ planId: parseInt(planId), procedureId: procedure.procedureId, userId });
      }

      for (const userId of toRemove) {
        await removeUser({ planId: parseInt(planId), procedureId: procedure.procedureId, userId });
      }

      setSelectedUsers(newSelected);
    } catch (err) {
      alert("Failed to update assignment");
      console.error(err);
    }
  };

  return (
    <div className="py-2">
        <div>
            {procedure.procedureTitle}      
      </div>

      <ReactSelect
        className="mt-2"
        placeholder="Select User to Assign"
        isMulti={true}
        options={users}
        value={selectedUsers}
        onChange={handleAssignUserToProcedure}
      />
    </div>
  );
};

export default PlanProcedureItem;
