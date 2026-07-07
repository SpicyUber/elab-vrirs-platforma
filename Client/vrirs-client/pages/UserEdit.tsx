import { useEffect, useState } from "react";
import {
  Box,
  Button,
  Stack,
  TextField,
  Typography,
  CircularProgress,
} from "@mui/material";

import { useParams, useNavigate } from "react-router-dom";
import { useUser } from "../hooks/useUser";
import { useServer } from "../hooks/useServer";
import Sidebar from "../components/Sidebar";

type UserProfileInfo = {
  id: string;
  fullName: string;
  email: string;
  indexNumber?: string;
  roles: string;
};

const EditUser = () => {
  const { userId } = useParams();
  const { user } = useUser();
  const { baseUrl } = useServer();
  const navigate = useNavigate();

  const [fullName, setFullName] = useState("");
  const [indexNumber, setIndexNumber] = useState("");

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    const load = async () => {
      const res = await fetch(
        `${baseUrl}/api/users/${userId}`,
        {
          headers: {
            Authorization: `Bearer ${user?.token}`,
          },
        }
      );

      if (!res.ok) {
        setLoading(false);
        return;
      }

      const data: UserProfileInfo = await res.json();

      setFullName(data.fullName);
      setIndexNumber(data.indexNumber ?? "");

      setLoading(false);
    };

    if (userId) {
      load();
    }
  }, [userId, user?.token, baseUrl]);


  const save = async () => {
    setSaving(true);

    try {
      const res = await fetch(
        `${baseUrl}/api/users`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user?.token}`,
          },
          body: JSON.stringify({
            id: userId,
            fullName,
            indexNumber,
          }),
        }
      );

      if (!res.ok) {
        alert("Failed to update user "+await res.text());
        return;
      }

      navigate("/users");

    } finally {
      setSaving(false);
    }
  };


  if (loading) {
    return <CircularProgress />;
  }


  return (
    <Box sx={{ padding: 3 }}>
        <Sidebar></Sidebar>
      <Typography variant="h4">
        Edit User
      </Typography>

      <Stack spacing={2} sx={{ mt: 3 }}>

        <TextField
          label="Full Name"
          value={fullName}
          onChange={(e) => setFullName(e.target.value)}
        />

        <TextField
          label="Index Number"
          value={indexNumber}
          onChange={(e) => setIndexNumber(e.target.value)}
        />


        <Button
          variant="contained"
          onClick={save}
          disabled={saving}
        >
          {saving ? "Saving..." : "Save"}
        </Button>

        <Button
          variant="outlined"
          onClick={() => navigate(-1)}
        >
          Cancel
        </Button>

      </Stack>
    </Box>
  );
};

export default EditUser;