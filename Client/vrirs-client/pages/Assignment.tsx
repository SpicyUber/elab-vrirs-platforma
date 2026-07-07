import { useEffect, useState } from "react";
import {
  Box,
  Card,
  CardContent,
  Chip,
  Stack,
  Typography,
  CircularProgress,
  Button,
  Divider
} from "@mui/material";
import Sidebar from "../components/Sidebar";
import { useUser } from "../hooks/useUser";
import { useServer } from "../hooks/useServer";
import type { AssignmentInfo } from "../types/AssignmentInfo";
import type { SubmissionInfo } from "../types/SubmissionInfo";
import { useNavigate, useParams } from "react-router-dom";
import type {AssignmentAssetInfo} from "../types/AssignmentAssetInfo";

const Assignment = () => {
  const { assignmentId } = useParams();
  const { user } = useUser();
  const { baseUrl } = useServer();
  const navigate = useNavigate();
  const [assignment, setAssignment] = useState<AssignmentInfo | null>(null);
  const [submissions, setSubmissions] = useState<SubmissionInfo[]>([]);
  const [assets, setAssets] = useState<AssignmentAssetInfo[]>([]);
  const [loading, setLoading] = useState(true);
  const [isStudent, setIsStudent] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchAssignment() {
      try {
        const res = await fetch(
          `${baseUrl}/api/assignments/${assignmentId}`,
          {
            headers: {
              Authorization: `Bearer ${user?.token}`,
            },
          }
        );

        if (!res.ok) {
          setError("Could not load assignment.");
          return;
        }

        const data: AssignmentInfo = await res.json();
        setAssignment(data);
      } catch {
        setError("Something went wrong while loading assignment.");
      } finally {
        setLoading(false);
      }
    }

    if (assignmentId && user?.token) {
      fetchAssignment();
    }
  }, [assignmentId, user?.token, baseUrl]);


  
  useEffect(() => {
    async function fetchSubmissions() {
      if (!assignment) return;

      try {
        const roleRes = await fetch(
          `${baseUrl}/api/courses/${assignment.courseId}/my-enrollment-role`,
          {
            headers: {
              Authorization: `Bearer ${user?.token}`,
            },
          }
        );

        if (!roleRes.ok) {
          setError("Could not determine role.");
          return;
        }

        const role = (await roleRes.json()) as string;

        const url =
          role.toLowerCase() === "student"
            ? `${baseUrl}/api/submissions/from-assignment/${assignmentId}/mine`
            : `${baseUrl}/api/submissions/from-assignment/${assignmentId}`;


         setIsStudent(role.toLowerCase() === "student");

        const res = await fetch(url, {
          headers: {
            Authorization: `Bearer ${user?.token}`,
          },
        });

        if (!res.ok) {
          setError("Could not load submissions.");
          return;
        }

        const data: SubmissionInfo[] = await res.json();
        setSubmissions(data);
      } catch {
        setError("Something went wrong while loading submissions.");
      }
    }

    if (assignment && user?.token) {
      fetchSubmissions();
    }
  }, [assignment, assignmentId, user?.token, baseUrl]);

 useEffect(() => {
  if (!assignment?.id || !user?.token) return;

  const fetchAssets = async () => {
    try {
      const res = await fetch(
        `${baseUrl}/api/assignments/${assignment.id}/assets`,
        {
          headers: {
            Authorization: `Bearer ${user?.token}`,
          },
        }
      );

      if (!res.ok) {
        const text = await res.text();
        console.error("Assets fetch failed:", res.status, text);
        setAssets([]);
        return;
      }

      const data = (await res.json()) as AssignmentAssetInfo[];
      setAssets(data);
    } catch (err) {
      console.error("Assets fetch error:", err);
      setError("Error while loading attachments");
    }
  };

  fetchAssets();
}, [assignment?.id, user?.token, baseUrl]);

const archive = async () => 
  {
    try{
      const res = await fetch(`${baseUrl}/api/assignments/${assignment?.id}`,
        {
          headers: {
            Authorization: `Bearer ${user?.token}`,

      },
      method : "DELETE"
    })
    }
    catch{
      setError("Error archiving the assignment.");
      return;
    }

     navigate(`/courses/${assignment?.courseId}`);
  }

const downloadFile = async (fileId: string, fileName: string) => {
  try {
    const res = await fetch(`${baseUrl}/api/downloads/${fileId}`, {
      headers: {
        Authorization: `Bearer ${user?.token}`,
      },
    });

    if (!res.ok) {
      console.error(await res.text());
      return;
    }

    const blob = await res.blob();
    const url = window.URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = fileName || "download";
    document.body.appendChild(a);
    a.click();

    a.remove();
    window.URL.revokeObjectURL(url);
  } catch (err) {
    console.error("Download failed:", err);
  }
};

 async function create()
{
  try{
    setLoading(true);
 const resp = await fetch(`${baseUrl}/api/submissions/from-assignment/${assignmentId}`, {headers: {"Authorization": "Bearer " + user?.token,}, method: "POST"});
 if(!resp.ok ) 
 {
  setError("Error creating submission."+await resp.text())
  return;
 }
 const submission = (await resp.json()) as SubmissionInfo;
 
 navigate(`/submissions/${submission.id}/edit`);
 }
 catch{
  setError("Error creating submission.")
  
 }
 
}

  if (loading) return <CircularProgress />;

  return (
    <Box>
      <Sidebar />

      <Box sx={{ padding: 3 }}>
        {error && <Typography color="error">{error}</Typography>}

        <Typography variant="h3">{assignment?.title}</Typography>

        {assignment?.status === "Draft" && <Typography color="text.secondary">
          {`--${assignment?.status}--`}
        </Typography>}

        <Typography color="text.secondary">
          {assignment?.description}
        </Typography>

        <Stack direction="row" spacing={1} sx={{ mt: 1 }}>
          <Chip label={assignment?.category} />
          {assignment?.allowProjectUpload && <Chip label={"Uploads Allowed"} />}
          {assignment?.allowMultipleAttempts && <Chip label={"Multiple Attempts Allowed"} />}
          <Chip label={assignment?.location} />
          <Chip label={`Points: ${assignment?.minPoints}-${assignment?.maxPoints}`} />
        </Stack>
          <Typography sx={{ mt: 1 }} variant="body2">
          Opens:{" "}
          {assignment?.opensAt
            ? new Date(assignment.opensAt + "Z").toLocaleString()
            : "---"}
        </Typography>
        <Typography sx={{ mt: 1 }} variant="body2">
          Due:{" "}
          {assignment?.dueAt
            ? new Date(assignment.dueAt + "Z").toLocaleString()
            : "---"}
        </Typography>
        
        {!isStudent && assignment?.status !== "Archived" && (
      <>
    <Button
      variant="contained"
      onClick={() => navigate("edit")} sx={{ margin: 2 }}>
      <Typography variant="caption">
          Edit
      </Typography>
    </Button>
    <Button
      variant="contained"
      onClick={archive} sx={{ margin: 2 }}>
      <Typography variant="caption">
          Archive
        </Typography>
    </Button>
    </>
  )}
     <Divider></Divider> 
        <Typography variant="h5" sx={{ mt: 4 }}>
          Attachments
        </Typography>
        <Typography color="text.secondary" >
            Attachment Count : {assets.length}
          </Typography>
<Stack spacing={2} sx={{ mt: 2 }} >
          {assets.map((a) => (
            <Card
  variant="outlined"
  key={a.assignmentId + a.fileMetadataId}
  onClick={() => downloadFile(a.fileMetadataId, a.fileName)}
  sx={{
    borderColor: "primary.main",
    backgroundColor: "rgba(25, 118, 210, 0.08)",
    cursor: "pointer",
    transition: "0.2s",
    "&:hover": {
      backgroundColor: "rgba(25, 118, 210, 0.15)",
      transform: "translateY(-1px)",
    },
  }}
>
              <CardContent>
                <Typography variant="h6">
                  {a.fileName}
                </Typography>
              </CardContent>
            </Card>
          ))}
        </Stack>
        <Divider></Divider>
        <Stack spacing={2} sx={{ mt: 2 }}>
        <Typography variant="h5" sx={{ mt: 4 }}>
          Submissions
        </Typography>

        {submissions.length === 0 && (
          <Typography color="text.secondary">
            No submissions yet.
          </Typography>
        )}
          {submissions.map((s) => (
            <Button onClick={() => navigate(`/submissions/${s.id}`)}>
            <Card key={s.id}>
              <CardContent>
                <Typography variant="h6">
                  {s.studentFullName}
                </Typography>

                <Typography>{s.title}</Typography>

                <Chip label={s.status} sx={{ mt: 1 }} />

                <Typography variant="caption">
                  {s.submittedAt
                    ? new Date(s.submittedAt + "Z").toLocaleString()
                    : ""}
                </Typography>
              </CardContent>
            </Card>
            </Button>
          ))}
          {isStudent && <Button onClick={create}>Add Submission</Button>}
        </Stack>
      </Box>
    </Box>
  );
};

export default Assignment;