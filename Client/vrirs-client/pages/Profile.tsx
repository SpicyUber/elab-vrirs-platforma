import { useEffect, useState } from "react";
import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Stack,
  Typography,
} from "@mui/material";

import { useUser } from "../hooks/useUser";
import { useServer } from "../hooks/useServer";
import type { UserProfileInfo } from "../types/UserProfileInfo";
import Sidebar from "../components/Sidebar";
import { useNavigate } from "react-router-dom";

const Profile = () => {
  const { user, logout } = useUser();
  const { baseUrl } = useServer();
  const navigate = useNavigate();

  const [profile, setProfile] = useState<UserProfileInfo | null>(null);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [file, setFile] = useState<File | null>(null);

  const loadProfile = async () => {
    if (!user) return;

    setLoading(true);

    try {
      const res = await fetch(`${baseUrl}/api/users/${user.id}`, {
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      });

      if (!res.ok) {
        alert("Failed to load profile.");
        return;
      }

      const data: UserProfileInfo = await res.json();
      setProfile(data);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProfile();
  }, [user]);

  const uploadAvatar = async () => {
    if (!file) return;

    setUploading(true);

    try {
      const form = new FormData();
      form.append("avatar", file);

      const res = await fetch(
        `${baseUrl}/api/users/avatar-upload`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${user?.token}`,
          },
          body: form,
        }
      );

      if (!res.ok) {
        alert(await res.text());
        return;
      }

      setFile(null);
      await loadProfile();
    } finally {
      setUploading(false);
    }
  };

  if (loading) {
    return <CircularProgress />;
  }

  if (!profile) {
    return <Typography>Profile not found.</Typography>;
  }

  return (
    <Box sx={{ p: 3 }}>
        <Sidebar></Sidebar>
      <Card>
        <CardContent>
          <Stack spacing={3} >

            <Avatar
              src={
                profile.avatarInBase64
                  ? `data:image/png;base64,${profile.avatarInBase64}`
                  : undefined
              }
              sx={{ width: 140, height: 140 }}
            />

            <Typography variant="h4">
              {profile.fullName}
            </Typography>

            <Typography>{profile.email}</Typography>

            <Typography>
              Index: {profile.indexNumber ?? "-"}
            </Typography>

            <Typography>
              Roles: {profile.roles}
            </Typography>

            <Button
              component="label"
              variant="outlined"
            >
              Choose New Avatar
              <input
                hidden
                type="file"
                accept="image/*"
                onChange={(e) =>
                  setFile(e.target.files?.[0] ?? null)
                }
              />
            </Button>

            {file && (
              <Typography variant="body2">
                {file.name}
              </Typography>
            )}

            <Button
              variant="contained"
              disabled={!file || uploading}
              onClick={uploadAvatar}
            >
              {uploading ? "Uploading..." : "Upload Avatar"}
            </Button>

            <Button
              variant="contained"
              color="error"
              onClick={()=>{logout();navigate("/login");}}
            >
              Logout
            </Button>

          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
};

export default Profile;