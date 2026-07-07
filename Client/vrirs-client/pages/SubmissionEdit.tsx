import { useEffect, useState } from "react";
import type { ProjectAssetInfo } from "../types/ProjectAssetInfo";
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  CircularProgress,
  Stack,
  Switch,
  FormControlLabel,
} from "@mui/material";

import { useParams, useNavigate } from "react-router-dom";
import { useUser } from "../hooks/useUser";
import { useServer } from "../hooks/useServer";

import type { SubmissionInfo } from "../types/SubmissionInfo";

const SubmissionEdit = () => {
  const { submissionId } = useParams();
  const { user } = useUser();
  const { baseUrl } = useServer();
  const navigate = useNavigate();

  const [submission, setSubmission] = useState<SubmissionInfo | null>(null);

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");

  const [publish, setPublish] = useState(false);

  const [files, setFiles] = useState<File[]>([]);

  const [assets, setAssets] = useState<ProjectAssetInfo[]>([]);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

const refreshAssets = async () => {
  if (!submissionId) return;

  const res = await fetch(
    `${baseUrl}/api/submissions/${submissionId}/assets`,
    {
      headers: {
        Authorization: `Bearer ${user?.token}`,
      },
    }
  );

  if (!res.ok) return;

  const data = (await res.json()) as ProjectAssetInfo[];
  setAssets(data);
};

  useEffect(() => {
    async function load() {
      try {
        const res = await fetch(
          `${baseUrl}/api/submissions/${submissionId}`,
          {
            headers: {
              Authorization: `Bearer ${user?.token}`,
            },
          }
        );

        if (!res.ok) {
          setError("Failed to load submission");
          return;
        }

        const data: SubmissionInfo = await res.json();

        setSubmission(data);
        setTitle(data.title);
        setDescription(data.description ?? "");
        await refreshAssets();
      } catch {
        setError("Something went wrong while loading submission");
      } finally {
        setLoading(false);
      }
    }

    if (submissionId && user?.token) {
      load();
    }
  }, [submissionId, user?.token, baseUrl]);

  const save = async () => {
    if (!submission) return;

    setSaving(true);

    try {
      const res = await fetch(`${baseUrl}/api/submissions`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user?.token}`,
        },
        body: JSON.stringify({
          id: submission.id,
          title,
          description,
          publish,
          userId: user?.id,
        }),
      });

      if (!res.ok) {
        const text = await res.text();
        setError(text || "Failed to save submission");
        return;
      }

      return true;
    } catch {
      setError("Error while saving submission");
      return false;
    } finally {
      setSaving(false);
    }
  };

  const uploadFiles  = async (): Promise<boolean | undefined> => {
    if (!submissionId || files.length === 0) return true;
    let ok = false;
    setUploading(true);

    try {
      for (const file of files) {
        const formData = new FormData();
        formData.append("asset", file);

        const res = await fetch(
          `${baseUrl}/api/submissions/${submissionId}/assets`,
          {
            method: "POST",
            headers: {
              Authorization: `Bearer ${user?.token}`,
            },
            body: formData,
          }
        );

        if (!res.ok) {
          const text = await res.text();
          setError(text || "Upload failed");
          return false;
        }
      }
      ok = true;
      setFiles([]);
      await refreshAssets();
      
    } catch {
      
      setError("Error uploading files");
      ok = false;
    } finally {
      setUploading(false);
      return ok;
    }
  };

  const handleSaveAll = async () => {
    const ok = await uploadFiles();
    if (ok) {
      await save();
      navigate(`/submissions/${submissionId}`);
    }
  };

  if (loading) return <CircularProgress />;

  return (
    <Box sx={{ padding: 3 }}>
      <Card>
        <CardContent>
          <Typography variant="h4" gutterBottom>
            Edit Submission
          </Typography>

          {error && (
            <Typography color="error" sx={{ mb: 2 }}>
              {error}
            </Typography>
          )}

          <Stack spacing={2}>
            <TextField
              label="Title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              fullWidth
            />

            <TextField
              label="Description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              fullWidth
              multiline
              minRows={4}
            />

            {/* PUBLISH TOGGLE */}
            <FormControlLabel
              control={
                <Switch
                  checked={publish}
                  onChange={(e) => setPublish(e.target.checked)}
                />
              }
              label={publish ? "Published" : "Draft"}
            />

            {/* FILE UPLOAD */}
            <Button variant="outlined" component="label">
              Upload Files
              <input
                hidden
                type="file"
                multiple
                onChange={(e) =>
                  setFiles(Array.from(e.target.files ?? []))
                }
              />
            </Button>

            {files.length > 0 && (
              <Typography variant="caption">
                {files.length} file(s) selected
              </Typography>
            )}
            <Typography variant="h6">
  Uploaded Files
</Typography>

<Stack spacing={1}>
  {assets.length === 0 ? (
    <Typography color="text.secondary">
      No files uploaded.
    </Typography>
  ) : (
    assets.map(asset => (
      <Box
        key={asset.fileMetadataId}
      >
        <Typography>{asset.fileName}</Typography>
      </Box>
    ))
  )}
</Stack>
            <Stack direction="row" spacing={2}>
              <Button
                variant="contained"
                onClick={handleSaveAll}
                disabled={saving || uploading}
              >
                {saving || uploading ? "Processing..." : "Save"}
              </Button>

              <Button variant="outlined" onClick={() => navigate(-1)}>
                Cancel
              </Button>
            </Stack>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
};

export default SubmissionEdit;