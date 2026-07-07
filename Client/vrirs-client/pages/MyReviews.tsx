import { useEffect, useState } from "react";
import { Box, Card, CardContent, Chip, Stack, Typography, CircularProgress } from "@mui/material";
import Sidebar from "../components/Sidebar";
import { useUser } from "../hooks/useUser";
import {useServer} from "../hooks/useServer";
import type {SubmissionReviewInfo} from "../types/SubmissionReviewInfo"

const MyReviews = () => {
  const { user } = useUser();
  const {baseUrl} = useServer(); 
  const [reviews, setReviews] = useState<SubmissionReviewInfo[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchReviews() {
      try {
        const res = await fetch(baseUrl + "/api/submissions/mine/reviews", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + user?.token,
          },
        });

        if (!res.ok) {
          setError("Could not load your project reviews.");
          return;
        }

        const data: SubmissionReviewInfo[] = await res.json();
        setReviews(data);
      } catch (e) {
        setError("Something went wrong while loading reviews.");
      } finally {
        setLoading(false);
      }
    }

    fetchReviews();
  }, [user?.token]);

  return (
    <Box>
      <Sidebar />

      <Box>
        <Typography variant="h5">
          Recent Submission Reviews
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
            {reviews.map((review) => (
              <Card key={review.reviewId} sx={{ width: 280 }}>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {review.submissionTitle}
                  </Typography>

                  <Chip
                    label={review.reviewStatus}
                    color={
                      review.reviewStatus === "Approved"
                        ? "success"
                        : review.reviewStatus === "Rejected"
                        ? "error"
                        : "default"
                    }
                    size="small"
                    sx={{ mb: 1 }}
                  />
                  <Typography variant="body2" color="text.secondary">
                    Points: {review.points}
                  </Typography>

                  <Typography variant="body2" color="text.secondary">
                    Reviewed by: {review.reviewedByUserFullName}
                  </Typography>

                  {review.reviewComment && (
                    <Typography variant="body2">
                      "{review.reviewComment}"
                    </Typography>
                  )}

                  <Typography variant="caption" color="text.secondary">
                    {new Date(review.reviewedAt+"Z").toLocaleDateString()}
                  </Typography>
                </CardContent>
              </Card>
            ))}
          </Stack>
        )}
      </Box>
    </Box>
  );
};

export default MyReviews;
