import { useEffect, useState } from "react";
import {
  Box,
  Button,
  CircularProgress,
  MenuItem,
  TextField,
  Typography,
  Stack,
} from "@mui/material";
import { useParams, useNavigate } from "react-router-dom";
import type { CourseInfo } from "../types/CourseInfo";
import type { CourseCategory } from "../types/CourseCategory";
import { useUser } from "../hooks/useUser";
import { useServer } from "../hooks/useServer";
const categories: CourseCategory[] = ["I", "II", "III", "IV", "MASTER"];

const CourseEdit = () => {
  const { courseId } = useParams();
  const { user } = useUser();
  const { baseUrl } = useServer();
  const navigate = useNavigate();

  const [course, setCourse] = useState<CourseInfo | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

    useEffect(() => {
    const fetchCourse = async () => {
      try {
        const res = await fetch(`${baseUrl}/api/courses/${courseId}`, {
          headers: {
            Authorization: "Bearer " + user?.token,
          },
        });

        if (!res.ok) {
          setError("Failed to load course");
          return;
        }

        const data: CourseInfo = await res.json();
        setCourse(data);
      } catch {
        setError("Something went wrong");
      } finally {
        setLoading(false);
      }
    };

    if (courseId && user?.token) fetchCourse();
  }, [courseId, baseUrl, user?.token]);

    const handleSave = async () => {
    if (!course || !user) return;

    setSaving(true);

    try {
      const res = await fetch(`${baseUrl}/api/courses/${course.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + user.token,
        },
        body: JSON.stringify({
          id: course.id,
          name: course.name,
          description: course.description,
          startDate: course.startDate,
          endDate: course.endDate,
          category: course.category,
        }),
      });

      if (!res.ok) {
        setError("Failed to save course");
        return;
      }

      const updated: CourseInfo = await res.json();
      setCourse(updated);
      navigate(`/courses/${updated.id}`);
    } catch {
      setError("Something went wrong");
    } finally {
      setSaving(false);
    }
  };

    if (loading) return <CircularProgress />;

  if (error) return <Typography color="error">{error}</Typography>;

  if (!course) return null;

    return (
    <Box sx={{ p: 4 }}>
      <Typography variant="h5">
        Edit Course
      </Typography>

      <Stack spacing={2}>
        <TextField
          label="Name"
          value={course.name}
          onChange={(e) =>
            setCourse({ ...course, name: e.target.value })
          }
        />

        <TextField
          label="Description"
          multiline
          rows={3}
          value={course.description ?? ""}
          onChange={(e) =>
            setCourse({ ...course, description: e.target.value })
          }
        />

        <TextField
          select
          label="Category"
          value={course.category}
          onChange={(e) =>
            setCourse({
              ...course,
              category: e.target.value as CourseCategory,
            })
          }
        >
          {categories.map((c) => (
            <MenuItem key={c} value={c}>
              {c}
            </MenuItem>
          ))}
        </TextField>

        <TextField
          type="date"
          label="Start Date"
          value={course.startDate.slice(0, 10)}
          onChange={(e) =>
            setCourse({
              ...course,
              startDate: new Date(e.target.value).toISOString(),
            })
          }
        />

        <TextField
          type="date"
          label="End Date"
          value={course.endDate.slice(0, 10)}
          onChange={(e) =>
            setCourse({
              ...course,
              endDate: new Date(e.target.value).toISOString(),
            })
          }
        />

        <Button
          variant="contained"
          onClick={handleSave}
          disabled={saving}
        >
          Save
        </Button>
      </Stack>
    </Box>
  );
};

export default CourseEdit;