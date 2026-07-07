import { useEffect, useState } from "react";
import { Box, Card, CardContent, Chip, Stack, Typography, CircularProgress, Button } from "@mui/material";
import Sidebar from "../components/Sidebar";
import { useUser } from "../hooks/useUser";
import {useServer} from "../hooks/useServer";
import type { SubmissionInfo } from "../types/SubmissionInfo"
import { useNavigate } from "react-router-dom";

const RecentSubmissions = () => {
  const { user } = useUser();
  const {baseUrl} = useServer(); 
  const [reviews, setReviews] = useState<SubmissionInfo[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    async function fetchSubmissions() {
      try {
        const res = await fetch(baseUrl + "/api/submissions/mine", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + user?.token,
          },
        });

        if (!res.ok) {
          setError("Could not load your submissions.");
          return;
        }

        const data: SubmissionInfo[] = await res.json();
        setReviews(data);
      } catch (e) {
        setError("Something went wrong while loading submissions.");
      } finally {
        setLoading(false);
      }
    }

    fetchSubmissions();
  }, [user?.token]);

  return (
    <Box>
      <Sidebar />

      <Box>
        <Typography variant="h5">
          Recent Submissions
        </Typography>

        {loading && <CircularProgress />}

        {!loading && error && (
          <Typography color="error">{error}</Typography>
        )}

        {!loading && !error && reviews.length === 0 && (
          <Typography color="text.secondary">No reviews yet.</Typography>
        )}

        {!loading && !error && reviews.length > 0 && (
          <Stack spacing={{ xs: 1, sm: 2 }}
  direction="row"
  useFlexGap
  sx={{ flexWrap: 'wrap', paddingLeft: 10, paddingRight: 10 }}>
            {reviews.map((submission) => (
              <Button onClick={()=>navigate("/submissions/"+submission.id)}>
              <Card key={submission.id} sx={{ width: 280 }}>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {submission.title}
                  </Typography>

                  <Chip
                    label={submission.status}
                    color={
                      submission.status === "Submitted"
                        ? "success"
                        : "default"
                    }
                    size="small"
                    sx={{ mb: 1 }}
                  />
                  <Typography variant="body2" color="text.secondary">
                    Assignment: {submission.assignmentTitle}
                  </Typography>

                  <Typography variant="body2" color="text.secondary">
                    {(submission.submittedAt == null)
                    ? "Not submited yet"
                      : "Submitted At: "+ new Date(submission.submittedAt).toLocaleDateString()
                  }
                  </Typography>
                </CardContent>
              </Card>
              </Button>
            ))}
          </Stack>
        )}
      </Box>
    </Box>
  );
};

export default RecentSubmissions;
