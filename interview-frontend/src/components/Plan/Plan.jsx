import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  addProcedureToPlan,
  getPlanProcedures,
  getProcedures,
  getUsers,
  getPlanAssignments, 
} from "../../api/api";

import Layout from "../Layout/Layout";
import ProcedureItem from "./ProcedureItem/ProcedureItem";
import PlanProcedureItem from "./PlanProcedureItem/PlanProcedureItem";

const Plan = () => {
  const { id } = useParams();
  const [procedures, setProcedures] = useState([]);
  const [planProcedures, setPlanProcedures] = useState([]);
  const [users, setUsers] = useState([]);
  const [assignments, setAssignments] = useState([]); 

  useEffect(() => {
    (async () => {
      const [procedures, planProcedures, users, assignments] = await Promise.all([
        getProcedures(),
        getPlanProcedures(id),
        getUsers(),
        getPlanAssignments(id)
      ]);

      const userOptions = users.map((u) => ({ label: u.name, value: u.userId }));

      setUsers(userOptions);
      setProcedures(procedures);
      setPlanProcedures(planProcedures);
      setAssignments(assignments); 
    })();
  }, [id]);

  const handleAddProcedureToPlan = async (procedure) => {
    const exists = planProcedures.some((p) => p.procedureId === procedure.procedureId);
    if (exists) return;

    await addProcedureToPlan(id, procedure.procedureId);
    setPlanProcedures((prev) => [
      ...prev,
      {
        planId: id,
        procedureId: procedure.procedureId,
        procedure: {
          procedureId: procedure.procedureId,
          procedureTitle: procedure.procedureTitle,
        },
      },
    ]);
  };

  return (
    <Layout>
      <div className="container pt-4">
        <div className="d-flex justify-content-center">
          <h2>OEC Interview Frontend</h2>
        </div>
        <div className="row mt-4">
          <div className="col">
            <div className="card shadow">
              <h5 className="card-header">Repair Plan</h5>
              <div className="card-body">
                <div className="row">
                  <div className="col">
                    <h4>Procedures</h4>
                    <div>
                      {procedures.map((p) => (
                        <ProcedureItem
                          key={p.procedureId}
                          procedure={p}
                          handleAddProcedureToPlan={handleAddProcedureToPlan}
                          planProcedures={planProcedures}
                        />
                      ))}
                    </div>
                  </div>
                  <div className="col">
                    <h4>Added to Plan</h4>
                    <div>
                      {planProcedures.map((p) => {
                        const assignedUserIds = assignments
                          .filter((a) => a.procedureId === p.procedure.procedureId)
                          .map((a) => a.userId);

                        return (
                          <PlanProcedureItem
                            key={p.procedure.procedureId}
                            procedure={p.procedure}
                            users={users}
                            assignedUserIds={assignedUserIds} 
                            planId={id} 
                          />
                        );
                      })}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Plan;
