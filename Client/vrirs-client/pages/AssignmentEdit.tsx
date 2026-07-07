import {
  Box,
  Button,
  Stack,
  TextField,
  Typography,
  Switch,
  FormControlLabel,
  Divider,
  MenuItem,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import type { AssignmentInfo } from "../types/AssignmentInfo";
import { useServer } from "../hooks/useServer";
import { useUser } from "../hooks/useUser";

type AssignmentAssetInfo = {
  assignmentId: string;
  fileMetadataId: string;
  fileName: string;
  assetType: string;
};

export default function AssignmentEditPage() {
  const { assignmentId } = useParams();
  const navigate = useNavigate();

  const [assignment, setAssignment] = useState<AssignmentInfo | null>(null);
  const [assets, setAssets] = useState<AssignmentAssetInfo[]>([]);
  const [file, setFile] = useState<File | null>(null);

  const [loading, setLoading] = useState(true);
  const { baseUrl } = useServer();
  const { user } = useUser();

  // ----------------------------
  // LOAD ASSIGNMENT + ASSETS
  // ----------------------------
  useEffect(() => {
    const load = async () => {
      const headers = {
        Authorization: `Bearer ${user?.token}`,
      };

      const [assignmentRes, assetsRes] = await Promise.all([
        fetch(`${baseUrl}/api/assignments/${assignmentId}`, { headers }),
        fetch(`${baseUrl}/api/assignments/${assignmentId}/assets`, { headers }),
      ]);

      if (!assignmentRes.ok) {
        setLoading(false);
        return;
      }

      const assignmentData =
        (await assignmentRes.json()) as AssignmentInfo;

      setAssignment(assignmentData);

      if (assetsRes.ok) {
        const assetsData =
          (await assetsRes.json()) as AssignmentAssetInfo[];
        setAssets(assetsData);
      }

      setLoading(false);
    };

    load();
  }, [assignmentId]);

  // ----------------------------
  // UPDATE HELPERS
  // ----------------------------
  const update = (key: keyof AssignmentInfo, value: any) => {
    if (!assignment) return;
    setAssignment({ ...assignment, [key]: value });
  };

  // ----------------------------
  // SAVE ASSIGNMENT
  // ----------------------------
  const submit = async () => {
    if (!assignment) return;
    if(assignment.opensAt==null || assignment.dueAt == null) return;
    const res = await fetch(
      `${baseUrl}/api/assignments/from-course/${assignment.courseId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user?.token}`,
        },
        body: JSON.stringify({
          assignmentId: assignment.id,
          title: assignment.title,
          description: assignment.description,
          location: assignment.location,
          category: assignment.category,
          opensAt: new Date(assignment.opensAt).toISOString(),
          dueAt: new Date(assignment.dueAt).toISOString(),
          publish: assignment.status === "Published",
          allowProjectUpload: assignment.allowProjectUpload,
          allowMultipleAttempts: assignment.allowMultipleAttempts,
          maxPoints: assignment.maxPoints,
          minPoints: assignment.minPoints,
        }),
      }
    );

    if (!res.ok) {
      alert("Failed to update assignment");
      return;
    }

    navigate(-1);
  };

  // ----------------------------
  // UPLOAD ASSET
  // ----------------------------
  const uploadAsset = async () => {
    if (!file) return;

    const form = new FormData();
    form.append("asset", file);

    const res = await fetch(
      `${baseUrl}/api/assignments/${assignmentId}/assets`,
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${user?.token}`,
        },
        body: form,
      }
    );

    if (!res.ok) {
      alert("Upload failed."+await res.text());
      return;
    }

    setFile(null);
    await refreshAssets();
  };

  // ----------------------------
  // DELETE ASSET
  // ----------------------------
  const deleteAsset = async (fileMetadataId: string) => {
    const res = await fetch(
      `${baseUrl}/api/assignments/${assignmentId}/assets/${fileMetadataId}`,
      {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${user?.token}`,
        },
      }
    );

    if (!res.ok) {
      alert("Delete failed");
      return;
    }

    setAssets((prev) =>
      prev.filter((a) => a.fileMetadataId !== fileMetadataId)
    );
  };

  // ----------------------------
  // REFRESH ASSETS
  // ----------------------------
  const refreshAssets = async () => {
    const res = await fetch(
      `${baseUrl}/api/assignments/${assignmentId}/assets`,
      {
        headers: {
          Authorization: `Bearer ${user?.token}`,
        },
      }
    );

    if (!res.ok) return;

    const data = (await res.json()) as AssignmentAssetInfo[];
    setAssets(data);
  };

  // ----------------------------
  // UI STATES
  // ----------------------------
  if (loading) {
    return <Typography>Loading...</Typography>;
  }

  if (!assignment) {
    return <Typography>Assignment not found</Typography>;
  }

  // ----------------------------
  // RENDER
  // ----------------------------
  return (
    <Box>
      <Typography variant="h4">Edit Assignment</Typography>

      <Stack spacing={2}>
        <TextField
          label="Title"
          value={assignment.title ?? ""}
          onChange={(e) => update("title", e.target.value)}
        />

        <TextField
          label="Description"
          value={assignment.description ?? ""}
          onChange={(e) => update("description", e.target.value)}
          multiline
          rows={3}
        />

        <TextField
          label="Min Points"
          type="number"
          value={assignment.minPoints ?? 0}
          onChange={(e) => update("minPoints", Number(e.target.value))}
        />

        <TextField
          label="Max Points"
          type="number"
          value={assignment.maxPoints ?? 0}
          onChange={(e) => update("maxPoints", Number(e.target.value))}
        />

        <TextField
          select
          label="Location"
          value={assignment.location ?? "Class"}
          onChange={(e) => update("location", e.target.value)}
        >
          <MenuItem value="Class">Class</MenuItem>
          <MenuItem value="Homework">Homework</MenuItem>
        </TextField>

        <TextField
          select
          label="Category"
          value={assignment.category ?? "Other"}
          onChange={(e) => update("category", e.target.value)}
        >
          <MenuItem value="Csmp">Csmp</MenuItem>
          <MenuItem value="Gpss">Gpss</MenuItem>
          <MenuItem value="Other">Other</MenuItem>
        </TextField>

        <TextField
          label="Opens At"
          type="datetime-local"
          value={assignment.opensAt ? assignment.opensAt.slice(0, 16) : ""}
          onChange={(e) => update("opensAt", e.target.value)}
        />

        <TextField
          label="Due At"
          type="datetime-local"
          value={assignment.dueAt ? assignment.dueAt.slice(0, 16) : ""}
          onChange={(e) => update("dueAt", e.target.value)}
        />

        <FormControlLabel
          control={
            <Switch
              checked={assignment.allowProjectUpload ?? false}
              onChange={(e) =>
                update("allowProjectUpload", e.target.checked)
              }
            />
          }
          label="Allow Project Upload"
        />

        <FormControlLabel
          control={
            <Switch
              checked={assignment.allowMultipleAttempts ?? false}
              onChange={(e) =>
                update("allowMultipleAttempts", e.target.checked)
              }
            />
          }
          label="Allow Multiple Attempts"
        />

        <FormControlLabel
          control={
            <Switch
              checked={assignment.status === "Published"}
              onChange={(e) =>
                update("status", e.target.checked ? "Published" : "Draft")
              }
            />
          }
          label="Published"
        />

        <Divider />

        {/* ---------------- ASSETS ---------------- */}
        <Typography variant="h6">Assets</Typography>

        <Stack spacing={1}>
          {assets.map((asset) => (
            <Box
              key={asset.fileMetadataId}
            >
              <Typography>{asset.fileName}</Typography>

              <Button
                color="error"
                onClick={() => deleteAsset(asset.fileMetadataId)}
              >
                Delete
              </Button>
            </Box>
          ))}
        </Stack>

        <Divider />

        <input
          type="file"
          onChange={(e) => setFile(e.target.files?.[0] ?? null)}
        />

        <Button
          variant="outlined"
          disabled={!file}
          onClick={uploadAsset}
        >
          Upload Asset
        </Button>

        <Divider />

        <Button variant="contained" onClick={submit}>
          Save Assignment
        </Button>
      </Stack>
    </Box>
  );
}