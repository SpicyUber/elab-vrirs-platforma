import { useEffect, useState } from "react";
import {
  Box,
  Card,
  CardContent,
  Chip,
  Stack,
  Typography,
  CircularProgress,
  Divider,
  Button,
  Dialog,
  DialogTitle,
  MenuItem,
  TextField,
  DialogContent,
  DialogActions
} from "@mui/material";

import Sidebar from "../components/Sidebar";
import { useUser } from "../hooks/useUser";
import { useServer } from "../hooks/useServer";
import { useNavigate, useParams } from "react-router-dom";

import type { SubmissionInfo } from "../types/SubmissionInfo";
import type { SubmissionReviewInfo } from "../types/SubmissionReviewInfo";
import type { ProjectAssetInfo } from "../types/ProjectAssetInfo";
import type {ReviewStatus} from "../types/ReviewStatus";

type AssignmentInfo = {
  id: string;
  courseId: string;
};

const Submission = () => {
  const { submissionId } = useParams();
  const { user } = useUser();
  const { baseUrl } = useServer();
  const navigate = useNavigate();

  const [submission, setSubmission] = useState<SubmissionInfo | null>(null);
  const [reviews, setReviews] = useState<SubmissionReviewInfo[]>([]);
  const [assets, setAssets] = useState<ProjectAssetInfo[]>([]);

  const [canEdit, setCanEdit] = useState(false);
  const [canReview, setCanReview] = useState(false);

  const [openReview, setOpenReview] = useState(false);
  const [reviewError, setReviewError] = useState<string | null>(null);

  const [reviewStatus, setReviewStatus] = useState<ReviewStatus>("Approved");
  const [reviewComment, setReviewComment] = useState("");
  const [points, setPoints] = useState(0);
  const [submittingReview, setSubmittingReview] = useState(false);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);


  function popupForReviews()
  {
    return (<Dialog open={openReview} onClose={() => setOpenReview(false)} fullWidth>
  <DialogTitle>Create Review</DialogTitle>

  <DialogContent>
    <Stack spacing={2} sx={{ mt: 1 }}>
      <TextField
        select
        label="Status"
        value={reviewStatus}
        onChange={(e) => setReviewStatus(e.target.value as ReviewStatus)}
      >
        <MenuItem value="Approved">Approved</MenuItem>
        <MenuItem value="Rejected">Rejected</MenuItem>
        <MenuItem value="NeedsRevision">NeedsRevision</MenuItem>
      </TextField>

      <TextField
        label="Points"
        type="number"
        value={points}
        onChange={(e) => setPoints(Number(e.target.value))}
      />

      <TextField
        label="Comment"
        multiline
        minRows={3}
        value={reviewComment}
        onChange={(e) => setReviewComment(e.target.value)}
      />
      {reviewError && <Typography color="error">{reviewError}</Typography>}
    </Stack>
  </DialogContent>

  <DialogActions>
    <Button onClick={() => setOpenReview(false)}>
      Cancel
    </Button>

    <Button
      variant="contained"
      onClick={submitReview}
      disabled={submittingReview}
    >
      {submittingReview ? "Submitting..." : "Submit"}
    </Button>
  </DialogActions>
</Dialog>)
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

  useEffect(() => {
    async function fetchAll() {
      try {
        const [subRes, revRes, assetRes] = await Promise.all([
          fetch(`${baseUrl}/api/submissions/${submissionId}`, {
            headers: { Authorization: `Bearer ${user?.token}` },
          }),
          fetch(`${baseUrl}/api/submissions/${submissionId}/reviews`, {
            headers: { Authorization: `Bearer ${user?.token}` },
          }),
          fetch(`${baseUrl}/api/submissions/${submissionId}/assets`, {
            headers: { Authorization: `Bearer ${user?.token}` },
          }),
        ]);

        if (!subRes.ok || !revRes.ok || !assetRes.ok) {
          setError("Failed to load submission data.");
          return;
        }

        const subData: SubmissionInfo = await subRes.json();
        const revData: SubmissionReviewInfo[] = await revRes.json();
        const assetData: ProjectAssetInfo[] = await assetRes.json();

        setSubmission(subData);
        setReviews(revData);
        setAssets(assetData);

        const assignmentRes = await fetch(
          `${baseUrl}/api/assignments/${subData.assignmentId}`,
          {
            headers: { Authorization: `Bearer ${user?.token}` },
          }
        );

        if (!assignmentRes.ok) {
          setError("Failed to load assignment.");
          return;
        }

        const assignmentData: AssignmentInfo = await assignmentRes.json();

        const roleRes = await fetch(
          `${baseUrl}/api/courses/${assignmentData.courseId}/my-enrollment-role`,
          {
            headers: { Authorization: `Bearer ${user?.token}` },
          }
        );

        let roleData = "";
        if (roleRes.ok) {
          roleData = await roleRes.json();
        }

        setCanReview(roleData.toLowerCase() === "teacher" && subData.status === "Submitted")

        setCanEdit(
          roleData.toLowerCase() === "student" && subData.status === "Draft"
        );
      } catch {
        setError("Something went wrong while loading submission.");
      } finally {
        setLoading(false);
      }
    }

    if (submissionId && user?.token) {
      fetchAll();
    }
  }, [submissionId, user?.token, baseUrl]);

  const submitReview = async () => {
  if (!submissionId) return;

  setSubmittingReview(true);

  try {
    const res = await fetch(
      `${baseUrl}/api/submissions/${submissionId}/reviews`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user?.token}`,
        },
        body: JSON.stringify({
          reviewStatus,
          reviewComment,
          points,
        }),
      }
    );

    if (!res.ok) {
      const text = await res.text();
      setReviewError(text || "Failed to submit review");
      return;
    }

    const newReview = await res.json() as SubmissionReviewInfo;
    if(user!=null)
    newReview.reviewedByUserFullName = user.fullName;
    setReviews((prev) => [...prev,newReview]);
    

    setReviewComment("");
    setPoints(0);
    setReviewStatus("Approved");
    setOpenReview(false);
  } catch {
    setReviewError("Error submitting review");
  } finally {
    setReviewComment("");
    setPoints(0);
    setReviewStatus("Approved");
    setSubmittingReview(false);
  }
};

  if (loading) return <CircularProgress />;

  return (
    <Box>
      <Sidebar />

      <Box sx={{ padding: 3 }}>
        {error && <Typography color="error">{error}</Typography>}

        <Typography variant="h3">{submission?.title}</Typography>

        <Typography color="text.secondary">
          {submission?.description}
        </Typography>

        <Stack direction="row" spacing={1} sx={{ mt: 1 }}>
          <Chip label={submission?.status} />
          <Chip
            label={`Submitted: ${
              submission?.submittedAt
                ? new Date(submission.submittedAt + "Z").toLocaleString()
                : "---"
            }`}
          />
        </Stack>

        <Divider sx={{ my: 2 }} />
        <Typography variant="h5">Assets</Typography>

        {assets.length === 0 && (
          <Typography color="text.secondary">
            No assets uploaded.
          </Typography>
        )}

        <Stack spacing={2} sx={{ mt: 2 }}>
          {assets.map((a) => (
            
            <Card key={a.fileMetadataId + a.submissionId}>
              <Button onClick={() => downloadFile(a.fileMetadataId, a.fileName)}>
              <CardContent>
                <Typography variant="h6">{a.fileName}</Typography>
                <Chip label={a.assetType} sx={{ mt: 1 }} />
                <Chip label={a.uploadStatus} sx={{ mt: 1, ml: 1 }} />
              </CardContent></Button>
            </Card>
           
          ))}
        </Stack>

        <Divider sx={{ my: 3 }} />
        <Typography variant="h5">Reviews</Typography>

        {reviews.length === 0 && (
          <Typography color="text.secondary">
            No reviews yet.
          </Typography>
        )}

        <Stack spacing={2} sx={{ mt: 2 }}>
          {reviews.map((r) => (
            <Card key={r.reviewId}>
              <CardContent>
                <Stack direction="row" spacing={1}>
                  <Chip label={r.reviewStatus} />
                  <Chip label={`Points: ${r.points}`} />
                  <Typography variant="caption">
                    {new Date(r.reviewedAt).toLocaleString()}
                  </Typography>
                </Stack>

                <Typography variant="subtitle2" sx={{ mt: 1 }}>
                  {r.reviewedByUserFullName}
                </Typography>

                <Typography sx={{ mt: 1 }}>
                  {r.reviewComment ?? "No comment"}
                </Typography>
              </CardContent>
            </Card>
          ))}
        </Stack>

        {canEdit && (
          <Button
            variant="contained"
            sx={{ mt: 3 }}
            onClick={() =>
              navigate(`/submissions/${submissionId}/edit`)
            }
          >
            Edit Submission
          </Button>
        )}
  {( canReview &&
        <Button
        variant="contained"
        sx={{ mt: 3 }}
        onClick={() => setOpenReview(true)}
        >
        Add Review
        </Button>)}
        </Box>
        {popupForReviews()}
        </Box>)
  
};

export default Submission;